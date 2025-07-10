using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.Repositories.Interfaces.ReviewManagement
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(int id);

        // دوال خاصة بالمرضى
        Task<IEnumerable<Review>> GetPatientReviewsAsync(int patientId);
        Task<Review> GetReviewForAppointmentAsync(int appointmentId);
        Task<bool> CanPatientReviewAppointmentAsync(int patientId, int appointmentId);

        // دوال خاصة بالأطباء
        Task<IEnumerable<Review>> GetDoctorReviewsAsync(int doctorId);
        Task<float> GetDoctorAverageRatingAsync(int doctorId);

        // دوال خاصة بالإدارة
        Task<IEnumerable<Review>> GetFilterReviewsAsync(int?DoctorId=null, int? PatientId=null, int? MinRating=null, int? MaxRating=null, DateTime? Datetime=null, string sortBy = "newest");

        // عمليات CRUD
        Task<int> AddAsync(Review review);
        Task<bool> UpdateAsync(Review review);
        Task<bool> DeleteAsync(int id);

        // أدوات مساعدة
        Task<bool> ExistsAsync(int id);

    }
}
