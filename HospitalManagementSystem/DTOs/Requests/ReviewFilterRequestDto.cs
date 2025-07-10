using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class ReviewFilterRequestDto
    {
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        [Range(1, 5)]
        public int? MinRating { get; set; }
        [Range(1, 5)]
        public int? MaxRating { get; set; }
        public DateTime? Datetime { get; set; }
        public string SortBy { get; set; } = "newest";




    }
}
