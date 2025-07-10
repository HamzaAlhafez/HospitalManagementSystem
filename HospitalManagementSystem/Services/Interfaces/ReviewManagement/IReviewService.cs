using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Services.Interfaces.ReviewManagement
{
    public interface IReviewService
    {
        Task<ReviewinfoResponseDto?> GetByIdAsync(int id);
        Task<ReviewinfoResponseDto> AddPatientReviewAsync(int patientId, CreateReviewRequestDto reviewDto);
        Task<ReviewinfoResponseDto?> UpdatePatientReviewAsync(int ReviewId, UpdateReviewRequestDto requestDto);
        Task<IEnumerable<AllReviewsResponseDto>> GetPatientReviewsAsync(int patientId);
        Task<IEnumerable<AllReviewsResponseDto>> GetDoctorReviewsAsync(int doctorId);
        Task<float> GetDoctorAverageRatingAsync(int doctorId);
        Task<IEnumerable<ReviewFilterResponsDto>> GetFilteredReviewsAsync(ReviewFilterRequestDto filter);
        Task<bool> DeleteAsync(int id);


    }
}
