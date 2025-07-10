using System.ComponentModel.DataAnnotations;
using System.Numerics;
using HospitalManagementSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]

    public class User
    {
        public int UserId { get; set; }
        [StringLength(50)]
        
        
        
        public string Username { get; set; }

        [StringLength(200)]
        public string PasswordHash { get; set; }

        [StringLength(100)]
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        

        // Navigation properties (Lazy Loading)
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Admin Admin { get; set; }

        public virtual  ICollection<RefreshToken> RefreshToken  { get; set; }

        
    }
}
