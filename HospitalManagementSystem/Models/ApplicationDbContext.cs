using System.Reflection;
using HospitalManagementSystem.Models.Entities;

using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
           

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //set relationship DeleteBehavior Restrict
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;

            }

            //relationships
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

           
            modelBuilder.Entity<User>()
                .HasOne(u => u.Doctor)
                .WithOne(d => d.User)
                .HasForeignKey<Doctor>(d => d.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Patient)
                .WithOne(p => p.User)
                .HasForeignKey<Patient>(p => p.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Admin)
                .WithOne(a => a.User)
                .HasForeignKey<Admin>(a => a.UserId);

            modelBuilder.Entity<User>()
         .HasMany(u => u.RefreshToken) 
         .WithOne(rt => rt.User)
         .HasForeignKey(rt => rt.UserId);

            modelBuilder.Entity<Appointment>()
      .HasOne(a => a.Doctor)
      .WithMany(d => d.Appointments)
      .HasForeignKey(a => a.DoctorId);

            modelBuilder.Entity<Appointment>()
        .HasOne(a => a.Patient)
        .WithMany(p => p.Appointments)
        .HasForeignKey(a => a.PatientId);
            //Date seed Role
            modelBuilder.Entity<Role>().HasData(new Role() { RoleId = 1, RoleName = "Admin" }, new Role() { RoleId = 2, RoleName = "Doctor" }, new Role() { RoleId = 3, RoleName = "Patient" });
            // Configure Review entity
            modelBuilder.Entity<Review>(entity =>
            {
                // Primary key
                entity.HasKey(r => r.Id);

                // Relationships
                entity.HasOne(r => r.Appointment)
    .WithOne(a => a.Review)    
    .HasForeignKey<Review>(r => r.AppointmentId);

                entity.HasOne(r => r.Patient)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(r => r.PatientId);
                    

                entity.HasOne(r => r.Doctor)
                    .WithMany(d => d.Reviews)
                    .HasForeignKey(r => r.DoctorId);

                modelBuilder.Entity<Review>()
     .Property(r => r.Rating)
     .HasPrecision(10, 2);



            });








        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Admin> Admins { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Review> Reviews { get; set; }





    }
}
