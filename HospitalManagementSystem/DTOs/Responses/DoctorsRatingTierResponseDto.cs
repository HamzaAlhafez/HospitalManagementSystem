namespace HospitalManagementSystem.DTOs.Responses
{
    public class DoctorsRatingTierResponseDto
    {
        public string DoctorName { get; set; }
        public decimal AvgRating { get; set; }
        public long RatingRank { get; set; }

    }
}
