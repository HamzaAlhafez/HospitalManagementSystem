using System.IdentityModel.Tokens.Jwt;
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.Services.Interfaces.Auth.Token
{
    public interface ITokenService
    {
       Task< JwtSecurityToken> CreateJwtToken(User user);
        
       
        RefreshToken GenerateRefreshToken();
       
    }
}
