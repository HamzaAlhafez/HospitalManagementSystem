using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models.Entities
{
    [Index(nameof(LicenseNumber), IsUnique = true)]
    public class Doctor
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
