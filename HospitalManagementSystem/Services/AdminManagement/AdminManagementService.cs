using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Enums;
using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Repositories.Interfaces.AdminManagement;
using HospitalManagementSystem.Repositories.Interfaces.UserManagement;
using HospitalManagementSystem.Services.Interfaces.AdminManagement;
using Serilog;




namespace HospitalManagementSystem.Services.AdminManagement
{
    public class AdminManagementService : IAdminManagementService
    {
        private readonly IUserManagementRespository _userManagementRespository;
        private readonly IAdminManagementRespository _adminManagementRespository;

        public AdminManagementService(
            IUserManagementRespository userManagementRespository,
            IAdminManagementRespository adminManagementRespository)
        {
            _userManagementRespository = userManagementRespository;
            _adminManagementRespository = adminManagementRespository;
        }

        /// <summary>
        /// Deletes an admin from the system
        /// </summary>
        /// <param name="id">ID of the admin to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAdminAsync(int id)
        {
            Log.Information("Starting admin deletion for ID: {AdminId}", id);
            return await _adminManagementRespository.DeleteAdminAsync(id);
        }

        /// <summary>
        /// Retrieves admin information by ID
        /// </summary>
        /// <param name="id">ID of the admin to retrieve</param>
        /// <returns>Admin response DTO or null if not found</returns>
        public async Task<AdminResponseDto?> GetAdminByIdAsync(int id)
        {
            Log.Information("Retrieving admin with ID: {AdminId}", id);

            var admin = await _adminManagementRespository.GetAdminByIdReadOnlyAsync(id);
            if (admin?.User == null)
            {
                Log.Warning("Admin not found with ID: {AdminId}", id);
                return null;
            }

            Log.Debug("Successfully retrieved admin with ID: {AdminId}", id);
            return new AdminResponseDto
            {
                Username = admin.User.Username,
                Email = admin.User.Email,
                AccessLevel = admin.AccessLevel,
                IsActive = admin.User.IsActive
            };
        }

        /// <summary>
        /// Retrieves a paged list of admins
        /// </summary>
        /// <param name="dto">Pagination parameters</param>
        /// <returns>Paged response containing admin DTOs</returns>
        public async Task<PagedResponseDto<AdminDto>> GetPagedAdminsAsync(PaginationRequestDto dto)
        {
            Log.Information("Retrieving paged admins. Page: {PageNumber}, Size: {PageSize}",
                dto.PageNumber, dto.PageSize);

            var (admins, totalCount) = await _adminManagementRespository.GetPagedAdminsAsync(
                dto.PageNumber, dto.PageSize);

            var totalPages = (int)Math.Ceiling(totalCount / (double)dto.PageSize);

            var adminsDtos = admins.Select(a => new AdminDto
            {
                AdminId = a.AdminId,
                AccessLevel = a.AccessLevel,
                User = new UserDto
                {
                    Username = a.User.Username,
                    Email = a.User.Email,
                    IsActive = a.User.IsActive
                }
            }).ToList();

            Log.Debug("Retrieved {AdminCount} admins out of {TotalCount}",
                adminsDtos.Count, totalCount);

            
return new PagedResponseDto<AdminDto>
{
    Items = adminsDtos,
    TotalCount = totalCount,
    TotalPages = totalPages,
    CurrentPage = dto.PageNumber,
    PageSize = dto.PageSize
};
        }

        /// <summary>
        /// Registers a new admin in the system
        /// </summary>
        /// <param name="registerDto">Admin registration data</param>
        /// <returns>Message response indicating success or failure</returns>
        public async Task<MessageResponseDto> RegisterAdminAsync(RegisterAdminDto registerDto)
        {
            Log.Information("Starting admin registration for username: {Username}", registerDto.Username);

            // Check if email exists
            if (await _userManagementRespository.IsEmailExistAsync(registerDto.Email))
            {
                Log.Warning("Registration failed - Email already exists: {Email}", registerDto.Email);
                return new MessageResponseDto { Message = "Email is already registered!", IsSuccess = false };
            }

            // Check if Username exists
            if (await _userManagementRespository.IsUsernameExistAsync(registerDto.Username))
            {
                Log.Warning("Registration failed - Username already exists: {Username}", registerDto.Username);
                return new MessageResponseDto { Message = "Username is already taken!", IsSuccess = false };
            }

            var user = new User()
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            var admin = new Admin()
            {
                AccessLevel = (int)AdminAccessLevel.FullAccess
            };

            var userRole = new UserRole()
            {
                RoleId = (int)SystemRoles.Admin
            };

            if (await _adminManagementRespository.RegisterAdminAsync(user, admin, userRole))
            {
                Log.Information("Admin registered successfully: {Username}", registerDto.Username);
                return new MessageResponseDto { Message = "Registration completed successfully", IsSuccess = true };
            }
            else
            {
                Log.Error("Admin registration failed for username: {Username}", registerDto.Username);
                return new MessageResponseDto { Message = "An error occurred during registration. Please try again", IsSuccess = false };
            }
        }

        /// <summary>
        /// Updates admin information
        /// </summary>
        /// <param name="id">ID of the admin to update</param>
        /// <param name="updateAdminRequest">Updated admin data</param>
        /// <returns>Update response with status and updated data</returns>
        public async Task<UpdateAdminResponseDto?> UpdateAdminAsync(int id, UpdateAdminRequestDto updateAdminRequest)
        {
            Log.Information("Starting admin update for ID: {AdminId}", id);

            // Check if email exists
            if (await _adminManagementRespository.IsEmailExistsIgnoringCurrentAdminAsync(updateAdminRequest.Email, id))
            {
                Log.Warning("Update failed - Email already exists: {Email}", updateAdminRequest.Email);
                return new UpdateAdminResponseDto { Message = "Email is already registered!", IsSuccess = false };
            }

            // Check if Username exists
            if (await _adminManagementRespository.IsUsernameExistsIgnoringCurrentAdminAsync(updateAdminRequest.Username, id))
            {
                Log.Warning("Update failed - Username already exists: {Username}", updateAdminRequest.Username);
                return new UpdateAdminResponseDto { Message = "Username is already taken!", IsSuccess = false };
            }

           
           var admin = await _adminManagementRespository.GetAdminByIdForUpdateAsync(id);
            if (admin == null)
            {
                Log.Warning("Update failed - Admin not found with ID: {AdminId}", id);
                return new UpdateAdminResponseDto
                {
                    IsSuccess = false,
                    Message = "Admin Not Found"
                };
            }

            admin.User.Username = updateAdminRequest.Username;
            admin.User.Email = updateAdminRequest.Email;
            admin.AccessLevel = updateAdminRequest.AccessLevel;

            var isSuccess = await _adminManagementRespository.UpdateAdminAsync(admin);
            if (isSuccess)
            {
                Log.Information("Admin updated successfully for ID: {AdminId}", id);
                return new UpdateAdminResponseDto
                {
                    IsSuccess = true,
                    Message = "Admin data has been updated successfully!",
                    Email = admin.User.Email,
                    Username = admin.User.Username,
                    AccessLevel = admin.AccessLevel,
                };
            }
            else
            {
                Log.Error("Admin update failed for ID: {AdminId}", id);
                return new UpdateAdminResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred while updating!"
                };
            }
        }
    }
}