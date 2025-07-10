namespace HospitalManagementSystem.DTOs.Responses
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public UserDto User { get; set; }
    }
}
