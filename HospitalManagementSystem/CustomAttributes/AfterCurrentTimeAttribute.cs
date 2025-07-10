using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.CustomAttributes
{
    public class AfterCurrentTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var now = DateTime.Now; 

            if (value is DateTime dateTimeValue && dateTimeValue <= now)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

    }
}
