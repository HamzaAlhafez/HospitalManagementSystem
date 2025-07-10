using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;

namespace HospitalManagementSystem.Services.Interfaces.UserManagement
{
    public interface IUserManagementService
    {
        public Task<MessageResponseDto> ChangePasswordAsync(int userid, ChangePasswordRequestDto changePasswordRequestDto);
        public Task<MessageResponseDto> DeactivateUserAsync(int userid);

      

    }
}
