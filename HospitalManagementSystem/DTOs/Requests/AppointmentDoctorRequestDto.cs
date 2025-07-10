using HospitalManagementSystem.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class AppointmentDoctorRequestDto
    {
        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        

        [Required(ErrorMessage = "Appointment date and time is required")]
        [AfterCurrentTime(ErrorMessage = "The appointment date and time must be strictly after the current moment")]
        public DateTime DateTime { get; set; }

        public string? Notes { get; set; }

    }
}
