using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class CreateDoctorRequestDto
    {
        [StringLength(50)]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }


        [StringLength(200, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Specialization is required")]
        public string Specialization { get; set; }
        [Required(ErrorMessage = "LicenseNumber is required")]
        public string LicenseNumber { get; set; }

    }
}
