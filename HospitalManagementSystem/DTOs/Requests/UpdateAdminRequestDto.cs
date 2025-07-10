using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class UpdateAdminRequestDto
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


    }
}
