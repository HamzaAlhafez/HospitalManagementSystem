using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class UpdatePatientRequestDto
    {
        [StringLength(50)]
        [Required(ErrorMessage = "Username is required")]

        public string Username { get; set; }





        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "DateOfBirth is required")]

        public DateTime DateOfBirth { get; set; }

        public string? InsuranceNumber { get; set; }


    }
}
