using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories.Interfaces.ReviewManagement;
using HospitalManagementSystem.Enums;
using Microsoft.EntityFrameworkCore;
using Serilog;






namespace HospitalManagementSystem.Repositories.ReviewManagement
{
    /// <summary>
    /// Repository for managing patient reviews in the hospital management system
    /// </summary>
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the ReviewRepository
        /// </summary>
        /// <param name="context">Application database context</param>
        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new review to the system
        /// </summary>
        /// <param name="review">Review entity to add</param>
        /// <returns>ID of the created review, or -1 if failed</returns>
        public async Task<int> AddAsync(Review review)
        {
            Log.Information("Adding new review for appointment ID: {AppointmentId}", review.AppointmentId);

            try
            {
                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();

                Log.Information("Successfully added review with ID: {ReviewId}", review.Id);
                return review.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add review for appointment ID: {AppointmentId}", review.AppointmentId);
                return -1;
            }
        }

        /// <summary>
        /// Checks if a patient can review a specific appointment
        /// </summary>
        /// <param name="patientId">ID of the patient</param>
        /// <param name="appointmentId">ID of the appointment</param>
        /// <returns>True if patient can review, false otherwise</returns>
        public async Task<bool> CanPatientReviewAppointmentAsync(int patientId, int appointmentId)
        {
            Log.Debug("Checking if patient {PatientId} can review appointment {AppointmentId}",
                patientId, appointmentId);

            try
            {
                var canReview = await _context.Appointments.AnyAsync(x =>
                    x.PatientId == patientId &&
                    x.AppointmentId == appointmentId &&
                    x.Status == AppointmentStatus.completed.ToString());

                Log.Debug("Patient review permission result: {CanReview}", canReview);
                return canReview;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking review permission for patient {PatientId} and appointment {AppointmentId}",
                    patientId, appointmentId);
                throw;
            }
        }

        /// <summary>
        /// Deletes a review from the system
        /// </summary>
        /// <param name="id">ID of the review to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            Log.Information("Attempting to delete review ID: {ReviewId}", id);

            try
            {
                var review = await _context.Reviews.FindAsync(id);
                if (review != null)
                {
                    _context.Reviews.Remove(review);
                    await _context.SaveChangesAsync();

                    Log.Information("Successfully deleted review ID: {ReviewId}", id);

                    
                return true;
                }

                Log.Warning("Review not found for deletion - ID: {ReviewId}", id);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete review ID: {ReviewId}", id);
                return false;
            }
        }

        /// <summary>
        /// Checks if a review exists in the system
        /// </summary>
        /// <param name="id">ID of the review to check</param>
        /// <returns>True if review exists, false otherwise</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            Log.Debug("Checking existence of review ID: {ReviewId}", id);

            try
            {
                var exists = await _context.Reviews.AnyAsync(x => x.Id == id);
                Log.Debug("Review ID {ReviewId} exists: {Exists}", id, exists);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking existence of review ID: {ReviewId}", id);
                return false;
            }
        }

        /// <summary>
        /// Gets all reviews in the system (not implemented)
        /// </summary>
        /// <returns>Collection of reviews</returns>
        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            Log.Warning("GetAllReviewsAsync method called but not implemented");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a review by its ID
        /// </summary>
        /// <param name="id">ID of the review to retrieve</param>
        /// <returns>Review entity if found, null otherwise</returns>
        public async Task<Review?> GetByIdAsync(int id)
        {
            Log.Debug("Fetching review by ID: {ReviewId}", id);

            try
            {
                var review = await _context.Reviews
                    .Include(d => d.Doctor)
                    .Include(p => p.Patient)
                    .Include(a => a.Appointment)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (review != null)
                {
                    Log.Debug("Successfully retrieved review ID: {ReviewId}", id);
                    return review;
                }

                Log.Warning("Review not found for ID: {ReviewId}", id);
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching review ID: {ReviewId}", id);
                return null;
            }
        }

        /// <summary>
        /// Calculates the average rating for a doctor
        /// </summary>
        /// <param name="doctorId">ID of the doctor</param>
        /// <returns>Average rating as float</returns>
        public async Task<float> GetDoctorAverageRatingAsync(int doctorId)
        {
            Log.Debug("Calculating average rating for doctor ID: {DoctorId}", doctorId);

            try
            {
                var ratings = await _context.Reviews
                    .Where(x => x.DoctorId == doctorId)
                    .Select(x => (float)x.Rating)
                    .ToListAsync();

                if (ratings == null || !ratings.Any())
                {
                    Log.Debug("No reviews found for doctor ID: {DoctorId}", doctorId);
                    return 0f;
                }

                var average = ratings.Average();
                Log.Debug("Average rating for doctor ID {DoctorId}: {AverageRating}", doctorId, average);
                return average;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error calculating average rating for doctor ID: {DoctorId}", doctorId);
                return 0f;
            }
        }

    
        /// <summary>
        /// Gets all reviews for a specific doctor
        /// </summary>
        /// <param name="doctorId">ID of the doctor</param>
        /// <returns>Collection of reviews</returns>
        public async Task<IEnumerable<Review>> GetDoctorReviewsAsync(int doctorId)
        {
            Log.Debug("Fetching reviews for doctor ID: {DoctorId}", doctorId);

            try
            {
                var doctorReviews = await _context.Reviews
                    .Include(p => p.Patient)
                    .Include(d => d.Doctor)
                    .Where(x => x.DoctorId == doctorId)
                    .ToListAsync();

                Log.Debug("Found {AppointmentCount} reviews for doctor ID: {DoctorId}", doctorReviews.Count, doctorId);
                return doctorReviews;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching reviews for doctor ID: {DoctorId}", doctorId);
                return new List<Review>();
            }
        }

        /// <summary>
        /// Gets filtered reviews based on various criteria
        /// </summary>
        /// <param name="DoctorId">Optional doctor ID filter</param>
        /// <param name="PatientId">Optional patient ID filter</param>
        /// <param name="MinRating">Optional minimum rating filter</param>
        /// <param name="MaxRating">Optional maximum rating filter</param>
        /// <param name="Datetime">Optional date filter</param>
        /// <param name="sortBy">Sorting method (newest, highest, lowest, oldest)</param>
        /// <returns>Filtered and sorted collection of reviews</returns>
        public async Task<IEnumerable<Review>> GetFilterReviewsAsync(int? DoctorId = null,
                                                                   int? PatientId = null,
                                                                   int? MinRating = null,
                                                                   int? MaxRating = null,
                                                                   DateTime? Datetime = null,
                                                                   string sortBy = "newest")
        {
            Log.Debug("Fetching filtered reviews with parameters - " +
                     "DoctorId: {DoctorId}, PatientId: {PatientId}, " +
                     "MinRating: {MinRating}, MaxRating: {MaxRating}, " +
                     "Date: {Date}, SortBy: {SortBy}",
                     DoctorId, PatientId, MinRating, MaxRating, Datetime, sortBy);

            try
            {
                var query = _context.Reviews
                    .Include(d => d.Doctor)
                    .Include(p => p.Patient)
                    .Include(a => a.Appointment)
                    .AsQueryable();

                if (DoctorId.HasValue)
                {
                    query = query.Where(r => r.DoctorId == DoctorId);
                    Log.Debug("Applied DoctorId filter: {DoctorId}", DoctorId);
                }

                if (PatientId.HasValue)
                {
                    query = query.Where(r => r.PatientId == PatientId);
                    Log.Debug("Applied PatientId filter: {PatientId}", PatientId);
                }

                if (MinRating.HasValue)
                {
                    query = query.Where(r => r.Rating >= MinRating);
                    Log.Debug("Applied MinRating filter: {MinRating}", MinRating);
                }

                if (MaxRating.HasValue)
                {
                    query = query.Where(r => r.Rating <= MaxRating);
                    Log.Debug("Applied MaxRating filter: {MaxRating}", MaxRating);
                }

                if (Datetime.HasValue)
                {
                    query = query.Where(r => r.Date >= Datetime.Value.Date);
                    Log.Debug("Applied Date filter: {Date}", Datetime.Value.Date);
                }
                query = sortBy.ToLower() switch
                {
                    "highest" => query.OrderByDescending(r => r.Rating),
                    "lowest" => query.OrderBy(r => r.Rating),
                    "oldest" => query.OrderBy(r => r.Date),
                    _ => query.OrderByDescending(r => r.Date)
                };




                Log.Debug("Applied sorting by: {SortBy}", sortBy);

                var results = await query.ToListAsync();
                Log.Debug("Returning {AppointmentCount} filtered reviews", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching filtered reviews");
                return new List<Review>();
            }
        }

        /// <summary>
        /// Gets all reviews by a specific patient
        /// </summary>
        /// <param name="patientId">ID of the patient</param>
        /// <returns>Collection of reviews</returns>
        public async Task<IEnumerable<Review>> GetPatientReviewsAsync(int patientId)
        {
            Log.Debug("Fetching reviews by patient ID: {PatientId}", patientId);

            try
            {
                var patientReviews = await _context.Reviews
                    .Include(p => p.Patient)
                    .Include(d => d.Doctor)
                    .Where(x => x.PatientId == patientId)
                    .ToListAsync();

                Log.Debug("Found {AppointmentCount} reviews by patient ID: {PatientId}", patientReviews.Count, patientId);
                return patientReviews;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching reviews by patient ID: {PatientId}", patientId);
                return new List<Review>();
            }
        }

        /// <summary>
        /// Gets a review for a specific appointment (not implemented)
        /// </summary>
        /// <param name="appointmentId">ID of the appointment</param>
        /// <returns>Review entity</returns>
        public async Task<Review> GetReviewForAppointmentAsync(int appointmentId)
        {
            Log.Warning("GetReviewForAppointmentAsync method called but not implemented for appointment ID: {AppointmentId}",
                appointmentId);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an existing review
        /// </summary>
        /// <param name="review">Review entity with updated information</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAsync(Review review)
        {
            Log.Information("Updating review ID: {ReviewId}", review.Id);

            try
            {
                _context.Reviews.Update(review);
                await _context.SaveChangesAsync();

                Log.Information("Successfully updated review ID: {ReviewId}", review.Id);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update review ID: {ReviewId}", review.Id);
                return false;
            }
        }
    }
}