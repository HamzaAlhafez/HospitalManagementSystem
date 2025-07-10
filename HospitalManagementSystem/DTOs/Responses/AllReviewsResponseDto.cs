namespace HospitalManagementSystem.DTOs.Responses
{
    public class AllReviewsResponseDto
    {
        public string DoctorUsername { get; set; }
        public string Specialization { get; set; }
        public decimal rating { get; set; }
       public  string? Text { get; set; }
    }
}
