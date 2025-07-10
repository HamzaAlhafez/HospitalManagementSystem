using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class CreatePatientDto
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


        [StringLength(200, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]

        public string ConfirmPassword { get; set; }

    }
}
