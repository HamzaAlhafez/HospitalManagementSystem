namespace HospitalManagementSystem.DTOs.Responses
{
    public class PatientResponseDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? InsuranceNumber { get; set; }


        public bool IsActive { get; set; }

    }
}
