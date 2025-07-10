using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Security.Claims;
using HospitalManagementSystem.Migrations;
using Serilog;








namespace HospitalManagementSystem.Repositories.Auth
{
    public class AuthRespository : IAuthRespository
    {
        private readonly ApplicationDbContext _context;

        public AuthRespository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the active refresh token for a user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>Active refresh token or null if not found</returns>
        public async Task<RefreshToken?> GetActiveRefreshTokenAsync(int userId)
        {
            Log.Information("Retrieving active refresh token for user ID: {UserId}", userId);

            try
            {
                var refreshToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(r => r.UserId == userId);

                if (refreshToken == null || !refreshToken.IsActive)
                {
                    Log.Warning("No active refresh token found for user ID: {UserId}", userId);
                    return null;
                }

                Log.Debug("Found active refresh token for user ID: {UserId}", userId);
                return refreshToken;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving active refresh token for user ID: {UserId}", userId);
                return null;
            }
        }

        /// <summary>
        /// Saves a new refresh token for a user
        /// </summary>
        /// <param name="refreshToken">Refresh token to save</param>
        /// <param name="userid">ID of the user</param>
        /// <exception cref="Exception">Throws when saving fails</exception>
        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken, int userid)
        {
            Log.Information("Saving new refresh token for user ID: {UserId}", userid);

            try
            {
                var newRefreshToken = new RefreshToken()
                {
                    UserId = userid,
                    Token = refreshToken.Token,
                    ExpiresOn = refreshToken.ExpiresOn,
                    CreatedOn = refreshToken.CreatedOn,
                    RevokedOn = null
                };

                await _context.AddAsync(newRefreshToken);
                await _context.SaveChangesAsync();

                Log.Debug("Successfully saved refresh token for user ID: {UserId}", userid);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save refresh token for user ID: {UserId}", userid);
                throw new Exception("Failed to add refresh token", ex);
            }
        }

        /// <summary>
        /// Revokes a refresh token
        /// </summary>
        /// <param name="token">Token to revoke</param>
        /// <returns>True if revocation was successful, false otherwise</returns>
        public async Task<bool> RevokeTokenAsync(string token)
        {
            Log.Information("Attempting to revoke token: {Token}", token);

            try
            {
                var refreshToken = await _context.RefreshTokens
                    .SingleOrDefaultAsync(t => t.Token == token);

                if (refreshToken == null || !refreshToken.IsActive)
                {
                    Log.Warning("Token not found or already inactive: {Token}", token);
                    return false;
                }

                refreshToken.RevokedOn = DateTime.Now;
                await _context.SaveChangesAsync();

                
Log.Debug("Successfully revoked token: {Token}", token);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error revoking token: {Token}", token);
                return false;
            }
        }
    }
}