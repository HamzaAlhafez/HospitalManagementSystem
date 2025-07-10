using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class UpdateDoctorRequestDto
    {
        
        [StringLength(50)]
        [Required(ErrorMessage = "Username is required")]

        public string Username { get; set; }





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
