namespace HospitalManagementSystem.DTOs.Responses
{
    public class AdminResponseDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int? AccessLevel { get; set; }

        public bool IsActive { get; set; }

    }
}
