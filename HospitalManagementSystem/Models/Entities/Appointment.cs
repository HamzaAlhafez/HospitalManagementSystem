using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HospitalManagementSystem.Enums;

namespace HospitalManagementSystem.Models.Entities
{
    public class Appointment
    {
        [Key]
       
        public int AppointmentId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime DateTime { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string Status { get; set; } = AppointmentStatus.pending.ToString(); // Default value

        public string? Notes { get; set; }

       

        
       
        public virtual Patient Patient { get; set; }

       
        public virtual Doctor Doctor { get; set; }
        public virtual Review Review { get; set; }


    }
}
