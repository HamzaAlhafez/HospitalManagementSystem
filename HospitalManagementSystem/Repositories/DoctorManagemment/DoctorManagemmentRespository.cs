using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories.Interfaces.DoctorManagemment;
using Microsoft.Data.SqlClient;
using System.Data;
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.Enums;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Serilog;








namespace HospitalManagementSystem.Repositories.DoctorManagemment
{
    /// <summary>
    /// Repository for managing doctor-related operations in the hospital management system.
    /// </summary>
    public class DoctorManagemmentRespository : IDoctorManagemmentRespository
    {
        private readonly ApplicationDbContext _context;
        

        /// <summary>
        /// Initializes a new instance of the DoctorManagementRepository.
        /// </summary>
        /// <param name="context">The application database context.</param>
        
        public DoctorManagemmentRespository(ApplicationDbContext context)
        {
            _context = context;
           
        }

        /// <summary>
        /// Creates a new doctor with associated user account and role.
        /// </summary>
        /// <param name="user">The user account information.</param>
        /// <param name="doctor">The doctor-specific information.</param>
        /// <param name="userRole">The role to assign to the user.</param>
        /// <returns>The ID of the newly created doctor, or -1 if creation failed.</returns>
        public async Task<int> CreateDoctorAsync(User user, Doctor doctor, UserRole userRole)
        {
            Log.Information("Starting doctor creation process for username: {Username}", user.Username);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Log.Debug("Adding user to database");
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                Log.Debug("Adding doctor with UserId: {UserId}", user.UserId);
                doctor.UserId = user.UserId;
                await _context.Doctors.AddAsync(doctor);
                await _context.SaveChangesAsync();

                Log.Debug("Assigning role to user");
                userRole.UserId = user.UserId;
                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();

                Log.Information("Successfully created doctor with ID: {DoctorId}", doctor.DoctorId);
                await transaction.CommitAsync();
                return doctor.DoctorId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create doctor. Rolling back transaction.");
                await transaction.RollbackAsync();
                return -1;
            }
        }

        /// <summary>
        /// Checks if a license number already exists in the system.
        /// </summary>
        /// <param name="LicenseNumber">The license number to check.</param>
        /// <returns>True if the license number exists, false otherwise.</returns>
        public async Task<bool> IsLicenseNumberExistAsync(string LicenseNumber)
        {
            Log.Debug("Checking if license number exists: {LicenseNumber}", LicenseNumber);

            try
            {
                var exists = await _context.Doctors.AnyAsync(u => u.LicenseNumber == LicenseNumber);
                Log.Debug("License number {LicenseNumber} exists: {Exists}", LicenseNumber, exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking license number existence");
                return false;
            }
        }

       
        /// <summary>
        /// Checks if a license number exists, ignoring the current doctor.
        /// </summary>
        /// <param name="LicenseNumber">The license number to check.</param>
        /// <param name="currentUserId">The ID of the current doctor to ignore.</param>
        /// <returns>True if the license number exists for another doctor, false otherwise.</returns>
        public async Task<bool> IsLicenseNumberExistsIgnoringCurrentDoctorAsync(string LicenseNumber, int currentUserId)
        {
            Log.Debug("Checking license number {LicenseNumber} for other doctors besides ID {DoctorId}",
                LicenseNumber, currentUserId);

            try
            {
                var exists = await _context.Doctors.AnyAsync(u =>
                    u.LicenseNumber == LicenseNumber && u.DoctorId != currentUserId);

                Log.Debug("License number exists for other doctors: {Exists}", exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking license number uniqueness");
                return false;
            }
        }

        /// <summary>
        /// Updates a doctor's information.
        /// </summary>
        /// <param name="doctor">The updated doctor information.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            Log.Information("Starting update for doctor ID: {DoctorId}", doctor.DoctorId);

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Doctors.Update(doctor);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    Log.Information("Successfully updated doctor ID: {DoctorId}", doctor.DoctorId);
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to update doctor ID: {DoctorId}", doctor.DoctorId);
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks if a username exists, ignoring the current doctor.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <param name="currentUserId">The ID of the current doctor to ignore.</param>
        /// <returns>True if the username exists for another user, false otherwise.</returns>
        public async Task<bool> IsUsernameExistsIgnoringCurrentDoctorAsync(string username, int currentUserId)
        {
            Log.Debug("Checking username {Username} for other users besides doctor ID {DoctorId}",
                username, currentUserId);

            try
            {
                var exists = await _context.Doctors
                    .Include(d => d.User)
                    .AnyAsync(u => u.User.Username == username && u.DoctorId != currentUserId);

                Log.Debug("Username exists for other users: {Exists}", exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking username uniqueness");
                return false;
            }
        }

        /// <summary>
        /// Checks if an email exists, ignoring the current doctor.
        /// </summary>
        /// <param name="Email">The email to check.</param>
        /// <param name="currentUserId">The ID of the current doctor to ignore.</param>

       
        /// <returns>True if the email exists for another user, false otherwise.</returns>
        public async Task<bool> IsEmailExistsIgnoringCurrentDoctorAsync(string Email, int currentUserId)
        {
            Log.Debug("Checking email {Email} for other users besides doctor ID {DoctorId}",
                Email, currentUserId);

            try
            {
                var exists = await _context.Doctors
                    .Include(d => d.User)
                    .AnyAsync(u => u.User.Email == Email && u.DoctorId != currentUserId);

                Log.Debug("Email exists for other users: {Exists}", exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking email uniqueness");
                return false;
            }
        }

        /// <summary>
        /// Deletes a doctor from the system.
        /// </summary>
        /// <param name="id">The ID of the doctor to delete.</param>
        /// <returns>True if the deletion was successful, false otherwise.</returns>
        public async Task<bool> DeleteDoctorAsync(int id)
        {
            Log.Information("Starting deletion process for doctor ID: {DoctorId}", id);

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var doctor = await _context.Doctors
                        .Include(u => u.User)
                        .FirstOrDefaultAsync(u => u.DoctorId == id);

                    if (doctor == null)
                    {
                        Log .Warning("Doctor with ID {DoctorId} not found for deletion", id);
                        return false;
                    }

                    Log.Debug("Removing doctor ID: {DoctorId}", id);
                    _context.Doctors.Remove(doctor);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    Log.Information("Successfully deleted doctor ID: {DoctorId}", id);
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to delete doctor ID: {DoctorId}", id);

                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        /// <summary>
        /// Retrieves a paginated list of doctors.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A tuple containing the list of doctors and the total count.</returns>
        public async Task<(List<Doctor> Doctors, int TotalCount)> GetPagedDoctorsAsync(int pageNumber, int pageSize)
        {
            Log.Debug("Retrieving paged doctors - Page: {PageNumber}, Size: {PageSize}",
                pageNumber, pageSize);

            try
            {
                var totalCountParam = new SqlParameter("@TotalCount", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                var doctors = await _context.Doctors
                    .FromSqlInterpolated($"EXEC Sp_PagingDoctor {pageNumber}, {pageSize}, {totalCountParam} OUTPUT")
                    .AsNoTracking()
                    .ToListAsync();

                int totalCount = (int)totalCountParam.Value;

                Log.Debug("Retrieved {AppointmentCount} doctors out of {TotalCount}",
                    doctors.Count, totalCount);

                return (doctors, totalCount);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving paged doctors");
                return (new List<Doctor>(), 0);
            }
        }

       
        /// <summary>
        /// Retrieves a doctor by ID for update purposes (tracked by context).
        /// </summary>
        /// <param name="id">The ID of the doctor to retrieve.</param>
        /// <returns>The doctor if found, null otherwise.</returns>
        public async Task<Doctor?> GetDoctorByIdForUpdateAsync(int id)
        {
            Log.Debug("Retrieving doctor ID {DoctorId} for update", id);

            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.User)
                    .SingleOrDefaultAsync(u => u.DoctorId == id);

                if (doctor == null)
                {
                    Log.Warning("Doctor ID {DoctorId} not found for update", id);
                }
                else
                {
                    Log.Debug("Found doctor ID {DoctorId} for update", id);
                }

                return doctor;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving doctor ID {DoctorId} for update", id);
                return null;
            }
        }

        /// <summary>
        /// Retrieves a doctor by ID in read-only mode (not tracked by context).
        /// </summary>
        /// <param name="id">The ID of the doctor to retrieve.</param>
        /// <returns>The doctor if found, null otherwise.</returns>
        public async Task<Doctor?> GetDoctorByIdReadOnlyAsync(int id)
        {
            Log.Debug("Retrieving doctor ID {DoctorId} in read-only mode", id);

            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.User)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.DoctorId == id);

                if (doctor == null)
                {
                    Log.Warning("Doctor ID {DoctorId} not found for read-only access", id);
                }
                else
                {
                    Log.Debug("Found doctor ID {DoctorId} for read-only access", id);
                }

                return doctor;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving doctor ID {DoctorId} in read-only mode", id);
                return null;
            }
        }
    }
}
