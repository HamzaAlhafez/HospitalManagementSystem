using HospitalManagementSystem.DTOs.databse;
using HospitalManagementSystem.DTOs.Internal;
using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.Repositories.Interfaces.StatsManagement
{
    public interface IStatsRepository
    {
        Task<IEnumerable<AppointmentStatusCountResultInternalDto>> GetAppointmentCountByStatus();
        Task<IEnumerable<DoctorAppointmentStatsResultInternalDto>> GetDoctorAppointmentStats();
        Task<IEnumerable<DoctorAppointmentStatsResultInternalDto>> GetCurrentMonthDoctorAppointments();
        Task<IEnumerable<DoctorsByRatingTierResultInternalDto>> GetDoctorsByRatingTier();

        Task<IEnumerable<PatientCountByAgeGroupResultInternalDto>> GetPatientCountByAgeGroup();

    }
}
