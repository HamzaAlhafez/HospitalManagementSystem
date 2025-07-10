namespace HospitalManagementSystem.Models.Entities
{
    public class UserRole
    {
        public int id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
