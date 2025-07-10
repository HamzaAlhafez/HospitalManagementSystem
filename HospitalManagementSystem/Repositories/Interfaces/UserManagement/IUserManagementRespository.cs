using System.Security.Claims;
using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.Repositories.Interfaces.UserManagement
{
    public interface IUserManagementRespository
    {
      Task<User?> GetUserByIdAsync(int id);
        Task<int> GetDoctorIdByUseridAsync(int id);
        Task<int> GetPatientIdByUseridAsync(int id);
         Task<bool> UpdateUserAsync(User user);
        Task<bool> IsEmailExistAsync(string Email);
        Task<bool> IsUsernameExistAsync(string Username);
            Task<bool> IsUsernameExistsIgnoringCurrentUserAsync(string username, int currentUserId);
        Task<bool> IsEmailExistsIgnoringCurrentUserAsync(string username, int currentUserId);
        Task<int> GetUserIdByTokenAsync(string token);
        Task<User?> ValidateCredentialsAsync(string Email, string Password);
       Task<List<Claim>> GetRolesAsync(int userid);







    }
}
