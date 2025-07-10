using HospitalManagementSystem.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class AppointmentPatientRequestDto
    {
        [Required(ErrorMessage = "Doctor ID is required")]
        public int Doctorid { get; set; }



        [Required(ErrorMessage = "Appointment date and time is required")]
        [AfterCurrentTime(ErrorMessage = "The appointment date and time must be strictly after the current moment")]
        public DateTime DateTime { get; set; }

        public string? Notes { get; set; }


    }
}
