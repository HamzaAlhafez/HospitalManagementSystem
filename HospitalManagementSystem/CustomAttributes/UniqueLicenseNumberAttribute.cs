using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.CustomAttributes
{
    public class UniqueLicenseNumberAttribute : ValidationAttribute
    {
        public async Task<ValidationResult?> IsValidAsync(
          object value,
          ValidationContext validationContext)
        {
            // الحصول على DbContext من مقدم الخدمة
            var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            if (value == null)
                return ValidationResult.Success;

            var licenseNumber = value.ToString();

            // التحقق بشكل غير متزامن من التكرار
            bool exists = await dbContext.Doctors
                .AnyAsync(d => d.LicenseNumber == licenseNumber);

            return exists
                ? new ValidationResult(ErrorMessage ?? "License number must be unique.")
                : ValidationResult.Success;
        }
        
    }

}


