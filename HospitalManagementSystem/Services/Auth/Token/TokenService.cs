using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Repositories.Interfaces.Auth;
using HospitalManagementSystem.Repositories.Interfaces.UserManagement;
using HospitalManagementSystem.Services.Interfaces.Auth.Token;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HospitalManagementSystem.Services.Auth.Token
{
    public class TokenService : ITokenService
    {
        private IOptions<JwtOptions> _jwtOptions;
       
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserManagementRespository _userManagementRespository;
        public TokenService(IOptions<JwtOptions> jwtoptions, IHttpContextAccessor httpContextAccessor, IUserManagementRespository userManagementRespository)
        {
            _jwtOptions = jwtoptions;
           
            _httpContextAccessor = httpContextAccessor;
            _userManagementRespository = userManagementRespository;
        }
        public async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var roles = await _userManagementRespository.GetRolesAsync(user.UserId);
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
     {
         new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
         new Claim(ClaimTypes.Name, user.Username),
         new Claim(ClaimTypes.Email, user.Email),
     }.Union(roles);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Value.Issuer,
                Audience = _jwtOptions.Value.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.SigningKey)),
                    SecurityAlgorithms.HmacSha256),
                Expires = DateTime.Now.AddMinutes(_jwtOptions.Value.LifetimeInMinutes),
                
                Subject = new ClaimsIdentity(claims)


            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return (JwtSecurityToken)securityToken;
        }






        public RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            RandomNumberGenerator.Fill(randomNumber);



            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
               

            };
        }

        public int GetUserIdFromExpiredToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return -1;

            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return -1;

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // قراءة الـ token دون التحقق من الصلاحية
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // البحث عن الـ claim باستخدام المفتاح الصحيح (مثال: "sub" أو "nameid")
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == JwtRegisteredClaimNames.Sub || // "sub"
                    c.Type == ClaimTypes.NameIdentifier       // "nameid"
                );

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    return userId;

                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
            
        

        
    }
}
