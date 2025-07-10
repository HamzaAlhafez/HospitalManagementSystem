using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories.Interfaces.AdminManagement;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using Serilog;







namespace HospitalManagementSystem.Repositories.AdminManagement
{
    /// <summary>
    /// Repository for managing admin-related operations in the Hospital Management System
    /// Handles CRUD operations for administrators with transaction support
    /// </summary>
    public class AdminManagementRespository : IAdminManagementRespository
    {
        private readonly ApplicationDbContext _context;
       

        /// <summary>
        /// Initializes a new instance of the AdminManagementRepository
        /// </summary>
        /// <param name="context">Database context for data access</param>
       
        public AdminManagementRespository(ApplicationDbContext context)
        {
            _context = context;
           
        }

        /// <summary>
        /// Deletes an admin from the system along with associated user record
        /// </summary>
        /// <param name="id">ID of the admin to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAdminAsync(int id)
        {
            Log.Information("Starting admin deletion for ID: {AdminId}", id);

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var admin = await _context.Admins.Include(u => u.User).FirstOrDefaultAsync(u => u.AdminId == id);
                    if (admin == null)
                    {
                        Log.Warning("Admin not found with ID: {AdminId}", id);
                        return false;
                    }

                    Log.Debug("Found admin with ID: {AdminId} for deletion", id);
                    _context.Admins.Remove(admin);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    Log.Information("Successfully deleted admin with ID: {AdminId}", id);
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error(ex, "Error occurred while deleting admin with ID: {AdminId}", id);
                    return false;
                }
            }
        }

        /// <summary>
        /// Retrieves an admin by ID for update operations (tracked by EF)
        /// </summary>
        /// <param name="id">ID of the admin to retrieve</param>
        /// <returns>Admin entity if found, null otherwise</returns>
        public async Task<Admin?> GetAdminByIdForUpdateAsync(int id)
        {
            Log.Information("Fetching admin for update with ID: {AdminId}", id);

            try
            {
                var admin = await _context.Admins
                       .Include(d => d.User)
                       .SingleOrDefaultAsync(u => u.AdminId == id);

                if (admin == null)
                {
                    Log.Warning("Admin not found for update with ID: {AdminId}", id);
                }
                else
                {
                    Log.Debug("Successfully retrieved admin for update with ID: {AdminId}", id);
                }

                return admin;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching admin for update with ID: {AdminId}", id);
                return null;
            }
        }

       
        /// <summary>
        /// Retrieves an admin by ID for read-only purposes (not tracked by EF)
        /// </summary>
        /// <param name="id">ID of the admin to retrieve</param>
        /// <returns>Admin entity if found, null otherwise</returns>
        public async Task<Admin?> GetAdminByIdReadOnlyAsync(int id)
        {
            Log.Information("Fetching admin (read-only) with ID: {AdminId}", id);

            try
            {
                var admin = await _context.Admins
                        .Include(d => d.User)
                        .AsNoTracking()
                        .SingleOrDefaultAsync(u => u.AdminId == id);

                if (admin == null)
                {
                    Log.Warning("Admin not found (read-only) with ID: {AdminId}", id);
                }
                else
                {
                    Log.Debug("Successfully retrieved admin (read-only) with ID: {AdminId}", id);
                }

                return admin;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching admin (read-only) with ID: {AdminId}", id);
                return null;
            }
        }

        /// <summary>
        /// Retrieves a paginated list of admins with total count
        /// </summary>
        /// <param name="pageNumber">Current page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Tuple containing list of admins and total count</returns>
        public async Task<(List<Admin> Admins, int TotalCount)> GetPagedAdminsAsync(int pageNumber, int pageSize)
        {
            Log.Information("Fetching paged admins. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

            try
            {
                var totalCountParam = new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var admins = await _context.Admins
                    .FromSqlInterpolated($"EXEC Sp_AdminsPaging {pageNumber}, {pageSize}, {totalCountParam} OUTPUT")
                    .AsNoTracking()
                    .ToListAsync();

                int totalCount = (int)totalCountParam.Value;

                Log.Information("Successfully retrieved {AdminCount} admins of {TotalCount} total", admins.Count, totalCount);
                return (admins, totalCount);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching paged admins. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
                return (new List<Admin>(), 0);
            }
        }

        /// <summary>
        /// Checks if an email exists in the system, ignoring the current admin
        /// </summary>
        /// <param name="email">Email to check</param>
        /// <param name="currentUserId">ID of the current admin to exclude from check</param>
        /// <returns>True if email exists, false otherwise</returns>
        public async Task<bool> IsEmailExistsIgnoringCurrentAdminAsync(string email, int currentUserId)
        {
            Log.Debug("Checking email existence: {Email}, ignoring admin ID: {CurrentUserId}", email, currentUserId);

            try
            {
                var exists = await _context.Admins.Include(d => d.User)
                    .AnyAsync(u => u.User.Email == email && u.AdminId != currentUserId);

                Log.Debug("Email {Email} exists (ignoring current user): {Exists}", email, exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking email existence: {Email}", email);
                return false;
            }
        }

       
        /// <summary>
        /// Checks if a username exists in the system, ignoring the current admin
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <param name="currentUserId">ID of the current admin to exclude from check</param>
        /// <returns>True if username exists, false otherwise</returns>
        public async Task<bool> IsUsernameExistsIgnoringCurrentAdminAsync(string username, int currentUserId)
        {
            Log.Debug("Checking username existence: {Username}, ignoring admin ID: {CurrentUserId}", username, currentUserId);

            try
            {
                var exists = await _context.Admins.Include(d => d.User)
                    .AnyAsync(u => u.User.Username == username && u.AdminId != currentUserId);

                Log.Debug("Username {Username} exists (ignoring current user): {Exists}", username, exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking username existence: {Username}", username);
                return false;
            }
        }

        /// <summary>
        /// Registers a new admin in the system with associated user and role
        /// </summary>
        /// <param name="user">User entity containing credentials</param>
        /// <param name="admin">Admin entity containing admin-specific data</param>
        /// <param name="userRole">UserRole entity defining the admin role</param>
        /// <returns>True if registration was successful, false otherwise</returns>
        public async Task<bool> RegisterAdminAsync(User user, Admin admin, UserRole userRole)
        {
            Log.Information("Starting admin registration for user: {Username}", user.Username);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync(); // Get UserId
                Log.Debug("User created with ID: {UserId}", user.UserId);

                admin.UserId = user.UserId;
                await _context.Admins.AddAsync(admin);
                await _context.SaveChangesAsync();
                Log.Debug("Admin profile created with ID: {AdminId}", admin.AdminId);

                userRole.UserId = user.UserId;
                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();
                Log.Debug("User role assigned for user ID: {UserId}", user.UserId);

                await transaction.CommitAsync();
                Log.Information("Successfully registered new admin with ID: {AdminId}", admin.AdminId);

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(ex, "Error occurred during admin registration for user: {Username}", user.Username);
                return false;
            }
        }

        /// <summary>
        /// Updates an existing admin record
        /// </summary>
        /// <param name="admin">Admin entity with updated values</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAdminAsync(Admin admin)
        {
            Log.Information("Starting update for admin ID: {AdminId}", admin.AdminId);

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Admins.Update(admin);
                    await _context.SaveChangesAsync();

                    
                   await transaction.CommitAsync();

                    Log.Information("Successfully updated admin with ID: {AdminId}", admin.AdminId);
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error(ex, "Error occurred while updating admin with ID: {AdminId}", admin.AdminId);
                    return false;
                }
            }
        }
    }
}
