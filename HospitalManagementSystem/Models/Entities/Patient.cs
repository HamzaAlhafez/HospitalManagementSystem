using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models.Entities
{
    [Index(nameof(InsuranceNumber), IsUnique = true)]
    public class Patient
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ?InsuranceNumber { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
