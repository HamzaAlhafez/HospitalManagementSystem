using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace HospitalManagementSystem.DTOs.Responses
{
    public class ReviewinfoResponseDto
    {
        public int reviewid { get; set; }
        public string Patientusername { get; set; }
        public string Doctorusername { get; set; }
        public decimal Rating { get; set; }
        public string? Text { get; set; }
        public DateTime Appointmentdatetime { get; set; }
        public DateTime DatetimeReview { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}


























    

