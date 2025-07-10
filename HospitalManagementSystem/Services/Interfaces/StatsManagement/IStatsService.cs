using HospitalManagementSystem.DTOs.Internal;
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.Services.Interfaces.StatsManagement
{
    public interface IStatsService
    {
        Task<IEnumerable<AppointmentStatusCountResponseDto>> GetAppointmentCountByStatus();
        Task<IEnumerable<DoctorAppointmentStatsResponseDto>> GetDoctorAppointmentStats();

        Task<IEnumerable<DoctorAppointmentStatsResponseDto>> GetCurrentMonthDoctorAppointments();

        Task<IEnumerable<DoctorsRatingTierResponseDto>> GetDoctorsByRatingTier();

        Task<IEnumerable<PatientCountByAgeResponseDto>> GetPatientCountByAgeGroup();
    }
}
