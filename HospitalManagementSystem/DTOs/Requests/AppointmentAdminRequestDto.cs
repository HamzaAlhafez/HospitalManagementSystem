using System.ComponentModel.DataAnnotations;
using HospitalManagementSystem.CustomAttributes;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class AppointmentAdminRequestDto
    {
        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Doctor ID is required")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Appointment date and time is required")]
        [AfterCurrentTime(ErrorMessage = "The appointment date and time must be strictly after the current moment")]
        public DateTime DateTime { get; set; }

        public string? Notes { get; set; }


    }
}
