using System.Text.Json.Serialization;

namespace HospitalManagementSystem.DTOs.Responses
{
    public class MessageResponseDto
    {
        
        public string? Message { get; set; }
        

        public bool IsSuccess { get; set; }

    }
}
