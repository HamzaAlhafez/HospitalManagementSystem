using System.Text.Json.Serialization;

namespace HospitalManagementSystem.DTOs.Responses
{
    public class TokenResponseDto
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }

        [JsonIgnore]
        public string? RefreshToken { get; set; }
        [JsonIgnore]

        public DateTime RefreshTokenExpiration { get; set; }


    }
}
