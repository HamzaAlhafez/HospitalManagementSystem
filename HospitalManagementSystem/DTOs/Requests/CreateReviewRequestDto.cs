using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class CreateReviewRequestDto
    {
        [Required]
        [Range(1, 5)]
        public decimal Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        [Required]
        public int AppointmentId { get; set; }


    }
}
