namespace HospitalManagementSystem.DTOs.Responses
{
    public class UpdateAdminResponseDto
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public string Username { get; set; }





        public string Email { get; set; }
        public int ?AccessLevel { get; set; }


    }
}
