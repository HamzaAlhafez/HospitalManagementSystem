namespace HospitalManagementSystem.DTOs.Responses
{
    public class AdminDto
    {
        public int AdminId { get; set; }
        public int? AccessLevel { get; set; }
        
        public UserDto User { get; set; }

    }
}
