using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs.Requests
{
    public class CancelRequestDto
    {
        [Required]
        public string CancellationReason { get; set; }

    }
}
