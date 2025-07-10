using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Repositories.Interfaces.AppointmentManagement;
using HospitalManagementSystem.Repositories.Interfaces.ReviewManagement;
using HospitalManagementSystem.Repositories.Interfaces.UserManagement;
using HospitalManagementSystem.Services.Interfaces.ReviewManagement;
using Serilog;


namespace HospitalManagementSystem.Services.ReviewManagement
{
    /// <summary>
    /// Service for managing patient reviews and ratings
    /// </summary>
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserManagementRespository _userManagementRespository;

        public ReviewService(IReviewRepository reviewRepository, IAppointmentRepository appointmentRepository, IUserManagementRespository userManagementRespository)
        {
            _reviewRepository = reviewRepository;
            _appointmentRepository = appointmentRepository;
            _userManagementRespository = userManagementRespository;
        }

        /// <summary>
        /// Adds a new patient review for a completed appointment
        /// </summary>
        /// <param name="userid">ID of the user submitting the review</param>
        /// <param name="reviewDto">Review data including appointment ID, rating and comment</param>
        /// <returns>Response containing review details or error message</returns>
        public async Task<ReviewinfoResponseDto> AddPatientReviewAsync(int userid, CreateReviewRequestDto reviewDto)
        {
            Log.Information("Starting to add review for user {UserId} on appointment {AppointmentId}", userid, reviewDto.AppointmentId);

            var patientid = await _userManagementRespository.GetPatientIdByUseridAsync(userid);
            if (patientid == -1)
            {
                Log.Warning("Failed to find patient ID for user {UserId}", userid);
                return new ReviewinfoResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to create appointment due to a database error."
                };
            }

            // Validate if patient can review this appointment
            if (!await _reviewRepository.CanPatientReviewAppointmentAsync(patientid, reviewDto.AppointmentId))
            {
                Log.Warning("Patient {PatientId} cannot review appointment {AppointmentId}", patientid, reviewDto.AppointmentId);
                return new ReviewinfoResponseDto
                {
                    IsSuccess = false,
                    Message = "You cannot review this appointment. Either the appointment isn't completed or doesn't belong to you."
                };
            }

            // Verify appointment exists
            var appointment = await _appointmentRepository.GetAppoitmentByIdforReadAsync(reviewDto.AppointmentId);
            if (appointment == null)
            {
                Log.Warning("Appointment {AppointmentId} not found", reviewDto.AppointmentId);
                return new ReviewinfoResponseDto
                {
                    IsSuccess = false,
                    Message = "The specified appointment doesn't exist."
                };
            }

            // Create review object
            var review = new Review()
            {
                AppointmentId = reviewDto.AppointmentId,
                PatientId = patientid,
                DoctorId = appointment.DoctorId,
                Rating = reviewDto.Rating,
                Text = string.IsNullOrWhiteSpace(reviewDto.Comment) ? null : reviewDto.Comment.Trim(),
                LastModifiedDate = null
            };



           

            // Attempt to add review
            var reviewId = await _reviewRepository.AddAsync(review);

            if (reviewId == -1)
            {
                Log.Error("Failed to save review for appointment {AppointmentId} by patient {PatientId}", reviewDto.AppointmentId, patientid);
                return new ReviewinfoResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to save your review. Please try again later."
                };
            }

            Log.Information("Successfully added review {ReviewId} for appointment {AppointmentId}", reviewId, reviewDto.AppointmentId);
            return new ReviewinfoResponseDto
            {
                reviewid = reviewId,
                Doctorusername = appointment.Doctor.User.Username,
                Patientusername = appointment.Patient.User.Username,
                Rating = reviewDto.Rating,
                Text = string.IsNullOrWhiteSpace(reviewDto.Comment) ? " No Text" : reviewDto.Comment,
                Appointmentdatetime = appointment.DateTime,
                DatetimeReview = DateTime.Now,
                IsSuccess = true,
                Message = "Thank you! Your review has been submitted successfully.",
            };
        }

        /// <summary>
        /// Deletes a review by its ID
        /// </summary>
        /// <param name="id">ID of the review to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            Log.Information("Attempting to delete review {ReviewId}", id);
            var result = await _reviewRepository.DeleteAsync(id);
            Log.Information("Delete operation for review {ReviewId} completed with result: {Result}", id, result);
            return result;
        }

        /// <summary>
        /// Retrieves review details by its ID
        /// </summary>
        /// <param name="id">ID of the review to retrieve</param>
        /// <returns>Review details or null if not found</returns>
        public async Task<ReviewinfoResponseDto?> GetByIdAsync(int id)
        {
            Log.Debug("Fetching review by ID: {ReviewId}", id);
            var review = await _reviewRepository.GetByIdAsync(id);

            if (review == null)
            {
                Log.Warning("Review {ReviewId} not found", id);
                return null;
            }

            Log.Information("Successfully retrieved review {ReviewId}", id);
            return new ReviewinfoResponseDto
            {
                reviewid = review.Id,
                Doctorusername = review.Doctor.User.Username,
                Patientusername = review.Patient.User.Username,
                Rating = review.Rating,
                Text = string.IsNullOrWhiteSpace(review.Text) ? " No Text" : review.Text,
                Appointmentdatetime = review.Appointment.DateTime,
                DatetimeReview = review.Date,
            };
        }

        /// <summary>
        /// Calculates the average rating for a doctor
        /// </summary>
        /// <param name="doctorId">ID of the doctor</param>
        /// <returns>Average rating as a float value</returns>
        public async Task<float> GetDoctorAverageRatingAsync(int doctorId)
        {
            Log.Information("Calculating average rating for doctor {DoctorId}", doctorId);
            var averageRating = await _reviewRepository.GetDoctorAverageRatingAsync(doctorId);
            Log.Information("Average rating for doctor {DoctorId} is {AverageRating}", doctorId, averageRating);
            return averageRating;
        }

      
        /// <summary>
        /// Retrieves all reviews for a specific doctor
        /// </summary>
        /// <param name="doctorId">ID of the doctor</param>
        /// <returns>Collection of reviews for the specified doctor</returns>
        public async Task<IEnumerable<AllReviewsResponseDto>> GetDoctorReviewsAsync(int doctorId)
        {
            Log.Information("Fetching all reviews for doctor {DoctorId}", doctorId);
            var doctorreview = await _reviewRepository.GetDoctorReviewsAsync(doctorId);

            Log.Information("Found {ReviewCount} reviews for doctor {DoctorId}", doctorreview.Count(), doctorId);
            return doctorreview.Select(p => new AllReviewsResponseDto
            {
                DoctorUsername = p.Doctor.User.Username,
                Specialization = p.Doctor.Specialization,
                rating = p.Rating,
                Text = p.Text
            });
        }

        /// <summary>
        /// Retrieves reviews based on specified filters
        /// </summary>
        /// <param name="filter">Filter criteria including doctor ID, patient ID, rating range, etc.</param>
        /// <returns>Collection of filtered reviews</returns>
        public async Task<IEnumerable<ReviewFilterResponsDto>> GetFilteredReviewsAsync(ReviewFilterRequestDto filter)
        {
            Log.Information("Filtering reviews with criteria: {@Filter}", filter);
            var reviews = await _reviewRepository.GetFilterReviewsAsync(
              DoctorId: filter.DoctorId,
              PatientId: filter.PatientId,
              MinRating: filter.MinRating,
              MaxRating: filter.MaxRating,
              Datetime: filter.Datetime,
              sortBy: filter.SortBy
            );

            Log.Information("Found {ReviewCount} reviews matching filter criteria", reviews.Count());
            var response = reviews.Select(r => new ReviewFilterResponsDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = string.IsNullOrWhiteSpace(r.Text) ? "Not comment" : r.Text,
                CreatedAt = r.Date,
                DoctorId = r.DoctorId,
                DoctorName = r.Doctor.User.Username,
                PatientId = r.PatientId,
                PatientName = r.Patient.User.Username,
                AppointmentId = r.AppointmentId,
                AppointmentDate = r.Appointment.DateTime,
            });

            return response;
        }

        /// <summary>
        /// Retrieves all reviews submitted by a specific patient
        /// </summary>
        /// <param name="userid">ID of the user (patient)</param>
        /// <returns>Collection of reviews submitted by the patient</returns>
        public async Task<IEnumerable<AllReviewsResponseDto>> GetPatientReviewsAsync(int userid)
        {
            Log.Information("Fetching reviews for user {UserId}", userid);
            var patientid = await _userManagementRespository.GetPatientIdByUseridAsync(userid);
            var patientreview = await _reviewRepository.GetPatientReviewsAsync(patientid);

            Log.Information("Found {ReviewCount} reviews for patient {PatientId}", patientreview.Count(), patientid);
            return patientreview.Select(p => new AllReviewsResponseDto
            {
                DoctorUsername = p.Doctor.User.Username,
                Specialization = p.Doctor.Specialization,
                rating = p.Rating,
                Text = p.Text
            });
        }

        /// <summary>
        /// Updates an existing patient review
        /// </summary>
        /// <param name="ReviewId">ID of the review to update</param>
        /// <param name="requestDto">Updated review data</param>
        /// <returns>Response indicating success or failure of the update</returns>

     
       public async Task<ReviewinfoResponseDto?> UpdatePatientReviewAsync(int ReviewId, UpdateReviewRequestDto requestDto)
        {
            Log.Information("Starting update for review {ReviewId}", ReviewId);
            var review = await _reviewRepository.GetByIdAsync(ReviewId);

            if (review == null)
            {
                Log.Warning("Review {ReviewId} not found for update", ReviewId);
                return null;
            }

            review.Text = string.IsNullOrWhiteSpace(requestDto.Comment) ? null : requestDto.Comment;
            review.Rating = requestDto.Rating;
            review.LastModifiedDate = DateTime.UtcNow;

            var isupdate = await _reviewRepository.UpdateAsync(review);

            if (!isupdate)
            {
                Log.Error("Failed to update review {ReviewId}", ReviewId);
                return new ReviewinfoResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to update the review. Please try again later."
                };
            }

            Log.Information("Successfully updated review {ReviewId}", ReviewId);
            return new ReviewinfoResponseDto
            {
                IsSuccess = true,
                Message = "Your review has been updated successfully."
            };
        }
    }
}