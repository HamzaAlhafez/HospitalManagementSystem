using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        [Range(1, 5)]
        [Precision(10, 2)]
        public decimal  Rating { get; set; }

        [MaxLength(500)]
        public string? Text { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public DateTime? LastModifiedDate { get; set; }

        // Navigation properties
        public virtual Appointment Appointment { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }




    }
}
