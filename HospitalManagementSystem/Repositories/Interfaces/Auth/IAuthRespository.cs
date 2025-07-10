using System.Security.Claims;
using Azure.Core;
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.Repositories.Interfaces.Auth
{
    public interface IAuthRespository
    {
      
        Task<RefreshToken?> GetActiveRefreshTokenAsync(int userId);
        Task SaveRefreshTokenAsync(RefreshToken refreshToken, int userid);
        Task<bool> RevokeTokenAsync(string token);










    }
}
