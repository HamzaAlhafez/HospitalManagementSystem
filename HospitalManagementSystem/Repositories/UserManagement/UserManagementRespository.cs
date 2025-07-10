using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories.Interfaces.UserManagement;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;




namespace HospitalManagementSystem.Repositories.UserManagement
{
    /// <summary>
    /// Repository for managing user-related data operations
    /// </summary>
    public class UserManagementRespository : IUserManagementRespository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the UserManagementRespository
        /// </summary>
        /// <param name="context">The database context</param>
        public UserManagementRespository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a user by their ID
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <returns>The user entity or null if not found</returns>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            Log.Debug("Fetching user by ID: {UserId}", id);

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    Log.Warning("User not found with ID: {UserId}", id);
                    return null;
                }

                Log.Debug("Successfully retrieved user with ID: {UserId}", id);
                return user;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching user with ID: {UserId}", id);
                return null;
            }
        }

        /// <summary>
        /// Gets the doctor ID associated with a user ID
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <returns>The doctor ID or -1 if not found</returns>
        public async Task<int> GetDoctorIdByUseridAsync(int id)
        {
            Log.Debug("Getting doctor ID for user ID: {UserId}", id);

            try
            {
                var doctor = await _context.Users
                    .Include(d => d.Doctor)
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (doctor == null || doctor.Doctor == null)
                {
                    Log.Warning("No doctor found for user ID: {UserId}", id);
                    return -1;
                }

                Log.Debug("Found doctor ID {DoctorId} for user ID: {UserId}", doctor.Doctor.DoctorId, id);
                return doctor.Doctor.DoctorId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting doctor ID for user ID: {UserId}", id);
                return -1;
            }
        }

        /// <summary>
        /// Checks if an email exists in the system
        /// </summary>
        /// <param name="Email">The email to check</param>
        /// <returns>True if the email exists, false otherwise</returns>
        public async Task<bool> IsEmailExistAsync(string Email)
        {
            Log.Debug("Checking email existence: {Email}", Email);

            try
            {
                var exists = await _context.Users.AnyAsync(u => u.Email == Email);
                Log.Debug("Email {Email} exists: {Exists}", Email, exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking email existence: {Email}", Email);
                return false;
            }
        }

        /// <summary>
        /// Checks if a username exists in the system

     
        /// </summary>
        /// <param name="Username">The username to check</param>
        /// <returns>True if the username exists, false otherwise</returns>
        public async Task<bool> IsUsernameExistAsync(string Username)
        {
            Log.Debug("Checking username existence: {Username}", Username);

            try
            {
                var exists = await _context.Users.AnyAsync(u => u.Username == Username);
                Log.Debug("Username {Username} exists: {Exists}", Username, exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking username existence: {Username}", Username);
                return true;
            }
        }

        /// <summary>
        /// Updates a user's information
        /// </summary>
        /// <param name="user">The user entity to update</param>
        /// <returns>True if the update was successful, false otherwise</returns>
        public async Task<bool> UpdateUserAsync(User user)
        {
            Log.Information("Updating user with ID: {UserId}", user.UserId);

            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                Log.Information("Successfully updated user with ID: {UserId}", user.UserId);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating user with ID: {UserId}", user.UserId);
                return false;
            }
        }

        /// <summary>
        /// Checks if a username exists for any user other than the current one
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <param name="currentUserId">The current user ID to exclude</param>
        /// <returns>True if the username exists for another user, false otherwise</returns>
        public async Task<bool> IsUsernameExistsIgnoringCurrentUserAsync(string username, int currentUserId)
        {
            Log.Debug("Checking username existence: {Username}, ignoring user ID: {UserId}", username, currentUserId);

            try
            {
                var exists = await _context.Users.AnyAsync(u => u.Username == username && u.UserId != currentUserId);
                Log.Debug("Username {Username} exists for other users: {Exists}", username, exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking username existence: {Username}", username);
                return true;
            }
        }

        /// <summary>
        /// Checks if an email exists for any user other than the current one
        /// </summary>
        /// <param name="Email">The email to check</param>
        /// <param name="currentUserId">The current user ID to exclude</param>
        /// <returns>True if the email exists for another user, false otherwise</returns>
        public async Task<bool> IsEmailExistsIgnoringCurrentUserAsync(string Email, int currentUserId)
        {
            Log.Debug("Checking email existence: {Email}, ignoring user ID: {UserId}", Email, currentUserId);

            try
            {
                var exists = await _context.Users.AnyAsync(u => u.Email == Email && u.UserId != currentUserId);
                Log.Debug("Email {Email} exists for other users: {Exists}", Email, exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking email existence: {Email}", Email);
                return true;
            }
        }

        /// <summary>
        /// Gets the user ID associated with a refresh token

   
        /// </summary>
        /// <param name="token">The refresh token</param>
        /// <returns>The user ID or -1 if not found</returns>
        public async Task<int> GetUserIdByTokenAsync(string token)
        {
            Log.Debug("Getting user ID by refresh token");

            try
            {
                var user = await _context.RefreshTokens.FirstOrDefaultAsync(T => T.Token == token);
                if (user == null)
                {
                    Log.Warning("No user found for refresh token");
                    return -1;
                }

                Log.Debug("Found user ID {UserId} for refresh token", user.UserId);
                return user.UserId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting user ID by refresh token");
                return -1;
            }
        }

        /// <summary>
        /// Gets all role claims for a user
        /// </summary>
        /// <param name="userid">The user ID</param>
        /// <returns>List of role claims</returns>
        public async Task<List<Claim>> GetRolesAsync(int userid)
        {
            Log.Debug("Getting roles for user ID: {UserId}", userid);

            try
            {
                var user = await _context.Users
                    .Include(d => d.UserRoles)
                    .ThenInclude(u => u.Role)
                    .SingleOrDefaultAsync(u => u.UserId == userid);

                if (user == null)
                {
                    Log.Warning("User not found when getting roles: {UserId}", userid);
                    return new List<Claim>();
                }

                var roleNames = user.UserRoles
                    .Select(ur => ur.Role.RoleName)
                    .Distinct();

                var claims = roleNames
                    .Select(role => new Claim(ClaimTypes.Role, role))
                    .ToList();

                Log.Debug("Found {RoleCount} roles for user ID: {UserId}", claims.Count, userid);
                return claims;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting roles for user ID: {UserId}", userid);
                return new List<Claim>();
            }
        }

        /// <summary>
        /// Validates user credentials
        /// </summary>
        /// <param name="Email">The user's email</param>
        /// <param name="Password">The user's password</param>
        /// <returns>The user entity if credentials are valid, null otherwise</returns>
        public async Task<User?> ValidateCredentialsAsync(string Email, string Password)
        {
            Log.Debug("Validating credentials for email: {Email}", Email);

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == Email);

                if (user == null)
                {
                    Log.Warning("No user found with email: {Email}", Email);
                    return null;
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash);
                Log.Debug("Password validation result for email {Email}: {IsValid}", Email, isPasswordValid);

                return isPasswordValid ? user : null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error validating credentials for email: {Email}", Email);
                return null;
            }
        }

        /// <summary>
        /// Gets the patient ID associated with a user ID
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <returns>The patient ID or -1 if not found</returns>

      
        public async Task<int> GetPatientIdByUseridAsync(int id)
        {
            Log.Debug("Getting patient ID for user ID: {UserId}", id);

            try
            {
                var patient = await _context.Users
                    .Include(d => d.Patient)
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (patient == null || patient.Patient == null)
                {
                    Log.Warning("No patient found for user ID: {UserId}", id);
                    return -1;
                }

                Log.Debug("Found patient ID {PatientId} for user ID: {UserId}", patient.Patient.PatientId, id);
                return patient.Patient.PatientId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting patient ID for user ID: {UserId}", id);
                return -1;
            }
        }
    }
}