using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.DTOs.Responses
{
    public class PatientDto
    {
        public int PatientId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? InsuranceNumber { get; set; }
        public UserDto User { get; set; }





    }
}
