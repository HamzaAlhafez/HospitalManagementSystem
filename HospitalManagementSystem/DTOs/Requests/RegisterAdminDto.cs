using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class RegisterAdminDto
    {
        [StringLength(50)]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        
        

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Range(1, 5, ErrorMessage = "AccessLevel must be between 1 and 5")]

        public int? AccessLevel { get; set; }
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
