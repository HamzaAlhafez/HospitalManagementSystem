using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.Repositories.Interfaces.AdminManagement
{
    public interface IAdminManagementRespository
    {
        Task<bool> RegisterAdminAsync(User user, Admin admin, UserRole userRole);
        Task<bool> UpdateAdminAsync(Admin admin);
        Task<Admin?> GetAdminByIdForUpdateAsync(int id);
        Task<Admin?> GetAdminByIdReadOnlyAsync(int id);
        Task<bool> IsUsernameExistsIgnoringCurrentAdminAsync(string username, int currentUserId);
        Task<bool> IsEmailExistsIgnoringCurrentAdminAsync(string Email, int currentUserId);

        Task<(List<Admin> Admins, int TotalCount)> GetPagedAdminsAsync(int pageNumber, int pageSize);

        Task<bool> DeleteAdminAsync(int id);



    }
}
