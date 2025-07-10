using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Repositories.Interfaces.UserManagement;
using HospitalManagementSystem.Services.Interfaces.UserManagement;
using Azure.Core;
using Serilog;





namespace HospitalManagementSystem.Services.UserManagement
{
    /// <summary>
    /// Service for managing user operations including password changes and account deactivation
    /// </summary>
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserManagementRespository _respository;

        /// <summary>
        /// Initializes a new instance of the UserManagementService
        /// </summary>
        /// <param name="respository">User management repository</param>
        public UserManagementService(IUserManagementRespository respository)
        {
            _respository = respository;
        }

        /// <summary>
        /// Changes a user's password after verifying the old password
        /// </summary>
        /// <param name="userid">ID of the user</param>
        /// <param name="changePasswordRequestDto">Password change request data</param>
        /// <returns>Response indicating success or failure</returns>
        public async Task<MessageResponseDto> ChangePasswordAsync(int userid, ChangePasswordRequestDto changePasswordRequestDto)
        {
            Log.Information("Starting password change for user ID: {UserId}", userid);

            var user = await _respository.GetUserByIdAsync(userid);

            // User not found
            if (user == null)
            {
                Log.Warning("User not found for password change: {UserId}", userid);
                return new MessageResponseDto
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
            }

            // Verify old password
            if (!BCrypt.Net.BCrypt.Verify(changePasswordRequestDto.OldPassword, user.PasswordHash))
            {
                Log.Warning("Old password verification failed for user ID: {UserId}", userid);
                return new MessageResponseDto
                {
                    Message = "Old password is incorrect.",
                    IsSuccess = false
                };
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordRequestDto.NewPassword);
            Log.Debug("Password hashed successfully for user ID: {UserId}", userid);

            // Save changes
            var updateResult = await _respository.UpdateUserAsync(user);

            if (updateResult)
            {
                Log.Information("Password changed successfully for user ID: {UserId}", userid);
                return new MessageResponseDto
                {
                    Message = "Password changed successfully.",
                    IsSuccess = true
                };
            }
            else
            {
                Log.Error("Failed to update password for user ID: {UserId}", userid);
                return new MessageResponseDto
                {
                    Message = "Failed to update password. Please try again.",
                    IsSuccess = false
                };
            }
        }

        /// <summary>
        /// Deactivates a user account
        /// </summary>
        /// <param name="userid">ID of the user to deactivate</param>
        /// <returns>Response indicating success or failure</returns>
        public async Task<MessageResponseDto> DeactivateUserAsync(int userid)
        {
            Log.Information("Starting deactivation for user ID: {UserId}", userid);

            var user = await _respository.GetUserByIdAsync(userid);

         
           // User not found
            if (user == null)
            {
                Log.Warning("User not found for deactivation: {UserId}", userid);
                return new MessageResponseDto
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
            }

            user.IsActive = false;
            Log.Debug("User marked as inactive: {UserId}", userid);

            // Save changes
            var updateResult = await _respository.UpdateUserAsync(user);

            if (updateResult)
            {
                Log.Information("User deactivated successfully: {UserId}", userid);
                return new MessageResponseDto
                {
                    Message = "User deactivated successfully.",
                    IsSuccess = true
                };
            }
            else
            {
                Log.Error("Failed to deactivate user: {UserId}", userid);
                return new MessageResponseDto
                {
                    Message = "Failed to deactivate user. Please try again.",
                    IsSuccess = false
                };
            }
        }
    }
}