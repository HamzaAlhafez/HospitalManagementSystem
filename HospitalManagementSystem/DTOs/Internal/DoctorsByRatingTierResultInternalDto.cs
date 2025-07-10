namespace HospitalManagementSystem.DTOs.Internal
{
    public class DoctorsByRatingTierResultInternalDto
    {
        public string Doctor_name { get; set; }
        public decimal Avg_Rating {  get; set; }
        public long Rating_Rank { get; set; }

    }
}
