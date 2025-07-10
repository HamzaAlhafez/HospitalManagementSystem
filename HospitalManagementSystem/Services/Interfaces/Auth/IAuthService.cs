using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.Services.Interfaces.Auth
{
    public interface  IAuthService
    {
    
        public Task<TokenResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        public Task<TokenResponseDto> RefreshTokenAsync(string token);
       public  Task<bool> RevokeTokenAsync(string token);

      

    }
}
