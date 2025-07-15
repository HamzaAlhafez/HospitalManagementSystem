using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class MailRequestDto
    {
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Required(ErrorMessage = "Email is required")]
        public string ToEmail { get; set; }
       
        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; }
        
        [Required(ErrorMessage = "Body is required")]
        public string Body { get; set; }
        public IList<IFormFile> ?Attachments { get; set; }



    }
}
