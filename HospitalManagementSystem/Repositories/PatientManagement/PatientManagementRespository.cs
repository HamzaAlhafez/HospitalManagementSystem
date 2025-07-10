using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories.Interfaces.PatientManagement;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data;






namespace HospitalManagementSystem.Repositories.PatientManagement
{
    /// <summary>
    /// Repository for managing patient data and operations
    /// </summary>
    public class PatientManagementRespository : IPatientManagementRespository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the PatientManagementRespository
        /// </summary>
        /// <param name="context">The database context</param>
        public PatientManagementRespository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new patient with associated user account and role
        /// </summary>
        /// <param name="user">User account details</param>
        /// <param name="patient">Patient details</param>
        /// <param name="userRole">User role assignment</param>
        /// <returns>True if creation was successful, false otherwise</returns>
        public async Task<bool> CreatePatientAsync(User user, Patient patient, UserRole userRole)
        {
            Log.Information("Starting patient creation for user {Username}", user.Username);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync(); // Get UserId

                patient.UserId = user.UserId;
                await _context.Patients.AddAsync(patient);
                await _context.SaveChangesAsync();

                userRole.UserId = user.UserId;
                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                Log.Information("Successfully created patient {PatientId} for user {UserId}", patient.PatientId, user.UserId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(ex, "Error creating patient for user {Username}", user.Username);
                return false;
            }
        }

        /// <summary>
        /// Deletes a patient by ID
        /// </summary>
        /// <param name="id">ID of the patient to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeletePatientAsync(int id)
        {
            Log.Information("Attempting to delete patient {PatientId}", id);

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var patient = await _context.Patients.Include(u => u.User).FirstOrDefaultAsync(u => u.PatientId == id);
                    if (patient == null)
                    {
                        Log.Warning("Patient {PatientId} not found for deletion", id);
                        return false;
                    }

                    _context.Patients.Remove(patient);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    Log.Information("Successfully deleted patient {PatientId}", id);
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error(ex, "Error deleting patient {PatientId}", id);
                    return false;
                }
            }
        }

       
        /// <summary>
        /// Retrieves paginated list of patients
        /// </summary>
        /// <param name="pageNumber">Current page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Tuple containing patient list and total count</returns>
        public async Task<(List<Patient> patients, int TotalCount)> GetPagedpatientsAsync(int pageNumber, int pageSize)
        {
            Log.Debug("Fetching paged patients - Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);

            try
            {
                var totalCountParam = new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var patients = await _context.Patients
                    .FromSqlInterpolated($"EXEC Sp_PatientsPaging {pageNumber}, {pageSize}, {totalCountParam} OUTPUT")
                    .AsNoTracking()
                    .ToListAsync();

                int totalCount = (int)totalCountParam.Value;

                Log.Information("Retrieved {PatientCount} patients out of {TotalCount} for page {PageNumber}",
                    patients.Count, totalCount, pageNumber);

                return (patients, totalCount);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving paged patients");
                return (new List<Patient>(), 0);
            }
        }

        /// <summary>
        /// Retrieves a patient by ID for update operations
        /// </summary>
        /// <param name="id">ID of the patient</param>
        /// <returns>Patient entity or null if not found</returns>
        public async Task<Patient?> GetPatientByIdForUpdateAsync(int id)
        {
            Log.Debug("Fetching patient {PatientId} for update", id);

            try
            {
                var patient = await _context.Patients
                    .Include(d => d.User)
                    .SingleOrDefaultAsync(u => u.PatientId == id);

                if (patient == null)
                {
                    Log.Warning("Patient {PatientId} not found for update", id);
                }
                else
                {
                    Log.Debug("Successfully retrieved patient {PatientId} for update", id);
                }

                return patient;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving patient {PatientId} for update", id);
                return null;
            }
        }

        /// <summary>
        /// Retrieves a patient by ID for read-only operations
        /// </summary>
        /// <param name="id">ID of the patient</param>
        /// <returns>Patient entity or null if not found</returns>
        public async Task<Patient?> GetPatientByIdReadOnlyAsync(int id)
        {
            Log.Debug("Fetching patient {PatientId} for read-only", id);

            try
            {
                var patient = await _context.Patients
                    .Include(d => d.User)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.PatientId == id);

                if (patient == null)
                {
                    Log.Warning("Patient {PatientId} not found for read-only", id);
                }

                return patient;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving patient {PatientId} for read-only", id);
                return null;
            }
        }

        /// <summary>
        /// Checks if an email exists for any patient other than the current one
        /// </summary>

      
        /// <param name="Email">Email to check</param>
        /// <param name="currentUserId">Current patient ID to exclude</param>
        /// <returns>True if email exists, false otherwise</returns>
        public async Task<bool> IsEmailExistsIgnoringCurrentPatientAsync(string Email, int currentUserId)
        {
            Log.Debug("Checking email existence: {Email}, ignoring patient {PatientId}", Email, currentUserId);

            try
            {
                var exists = await _context.Patients.Include(d => d.User)
                    .AnyAsync(u => u.User.Email == Email && u.PatientId != currentUserId);

                Log.Debug("Email {Email} check result: {Exists}", Email, exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking email existence: {Email}", Email);
                return false;
            }
        }

        /// <summary>
        /// Checks if an insurance number exists
        /// </summary>
        /// <param name="InsuranceNumber">Insurance number to check</param>
        /// <returns>True if insurance number exists, false otherwise</returns>
        public async Task<bool> IsInsuranceNumberExistAsync(string InsuranceNumber)
        {
            Log.Debug("Checking insurance number existence: {InsuranceNumber}", InsuranceNumber);

            try
            {
                if (string.IsNullOrEmpty(InsuranceNumber))
                {
                    Log.Debug("Insurance number is null or empty");
                    return false;
                }

                var exists = await _context.Patients
                    .AnyAsync(u => u.InsuranceNumber != null && u.InsuranceNumber == InsuranceNumber);

                Log.Debug("Insurance number {InsuranceNumber} check result: {Exists}", InsuranceNumber, exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking insurance number existence: {InsuranceNumber}", InsuranceNumber);
                return false;
            }
        }

        /// <summary>
        /// Checks if an insurance number exists for any patient other than the current one
        /// </summary>
        /// <param name="insuranceNumber">Insurance number to check</param>
        /// <param name="currentUserId">Current patient ID to exclude</param>
        /// <returns>True if insurance number exists, false otherwise</returns>
        public async Task<bool> IsInsuranceNumberExistsIgnoringCurrentPatientAsync(string? insuranceNumber, int currentUserId)
        {
            Log.Debug("Checking insurance number existence: {InsuranceNumber}, ignoring patient {PatientId}",
                insuranceNumber, currentUserId);

            try
            {
                if (string.IsNullOrEmpty(insuranceNumber))
                {
                    Log.Debug("Insurance number is null or empty");
                    return false;
                }

                var exists = await _context.Patients
                    .AnyAsync(p => p.InsuranceNumber != null &&
                                 p.InsuranceNumber == insuranceNumber &&
                                 p.PatientId != currentUserId);

                Log.Debug("Insurance number {InsuranceNumber} check result: {Exists}", insuranceNumber, exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking insurance number existence: {InsuranceNumber}", insuranceNumber);
                return false;
            }
        }

        /// <summary>
        /// Checks if a username exists for any patient other than the current one

 
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <param name="currentUserId">Current patient ID to exclude</param>
        /// <returns>True if username exists, false otherwise</returns>
        public async Task<bool> IsUsernameExistsIgnoringCurrentPatientAsync(string username, int currentUserId)
        {
            Log.Debug("Checking username existence: {Username}, ignoring patient {PatientId}", username, currentUserId);

            try
            {
                var exists = await _context.Patients.Include(d => d.User)
                    .AnyAsync(u => u.User.Username == username && u.PatientId != currentUserId);

                Log.Debug("Username {Username} check result: {Exists}", username, exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking username existence: {Username}", username);
                return false;
            }
        }

        /// <summary>
        /// Updates an existing patient record
        /// </summary>
        /// <param name="patient">Patient data to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            Log.Information("Starting update for patient {PatientId}", patient.PatientId);

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Patients.Update(patient);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    Log.Information("Successfully updated patient {PatientId}", patient.PatientId);
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error(ex, "Error updating patient {PatientId}", patient.PatientId);
                    return false;
                }
            }
        }
    }
}
