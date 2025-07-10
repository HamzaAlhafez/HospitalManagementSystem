namespace HospitalManagementSystem.Models.Entities
{
    public class Admin
    {
        public int AdminId { get; set; }
        public int UserId { get; set; }
        public int ?AccessLevel { get; set; }

        public virtual User User { get; set; }
    }
}
