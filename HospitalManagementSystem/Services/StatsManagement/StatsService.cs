using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using HospitalManagementSystem.DTOs.databse;
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Repositories.Interfaces.StatsManagement;
using HospitalManagementSystem.Services.Interfaces.StatsManagement;
using Serilog;

namespace HospitalManagementSystem.Services.StatsManagement
{
    /// <summary>
    /// Service for handling statistics-related operations in the hospital management system.
    /// </summary>


    public class StatsService : IStatsService
    {
       



        private readonly IStatsRepository _statsRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="StatsService"/> class.
        /// </summary>
        /// <param name="statsRepository">The statistics repository dependency.</param>

        public StatsService(IStatsRepository statsRepository)
        {
            _statsRepository = statsRepository;

        }
        /// <summary>
        /// Retrieves the count of appointments grouped by their current status.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="AppointmentStatusCountResponseDto"/> objects 
        /// containing the count and status of appointments.
        /// </returns>

        public async Task<IEnumerable<AppointmentStatusCountResponseDto>> GetAppointmentCountByStatus()
        {
            const string methodName = nameof(GetAppointmentCountByStatus);
            var repositoryResult = await _statsRepository.GetAppointmentCountByStatus();
            var result = repositoryResult.Select(x => new AppointmentStatusCountResponseDto
            {
                AppointmentCount = x.Appointment_count,  
                Status = x.Status
            }).ToList();
            Log.Information("Successfully retrieved {AppointmentCount} status groups", result.Count);

            return result;



        }
        /// <summary>
        /// Retrieves doctor appointment statistics In Current Month and maps them to the response DTO format.
        /// </summary>
        /// <returns>
        /// A list of <see cref="DoctorAppointmentStatsResponseDto"/> containing:
        /// <list type="bullet">
        ///   <item><description>AppointmentCount: Number of appointments</description></item>
        ///   <item><description>DoctorUsername: Name of the doctor</description></item>
        ///   <item><description>AppointmentStatus: Current status of appointments</description></item>
        /// </list>
        /// </returns>

        public async Task<IEnumerable<DoctorAppointmentStatsResponseDto>> GetCurrentMonthDoctorAppointments()
        {
            var repositoryResult = await _statsRepository.GetCurrentMonthDoctorAppointments();
            Log.Information("Successfully retrieved {Count} doctor appointment stats In CurrentMonth", repositoryResult.Count());
            var result = repositoryResult.Select(x => new DoctorAppointmentStatsResponseDto
            {
                AppointmentCount = x.Appointment_count,
                DoctorUsername = x.Username,
                AppointmentStatus = x.Status
            }).ToList();
            Log.Information("Mapped {Count} items to response DTO", result.Count);
            return result;

        }

        /// <summary>
        /// Retrieves doctor appointment statistics and maps them to the response DTO format.
        /// </summary>
        /// <returns>
        /// A list of <see cref="DoctorAppointmentStatsResponseDto"/> containing:
        /// <list type="bullet">
        ///   <item><description>AppointmentCount: Number of appointments</description></item>
        ///   <item><description>DoctorUsername: Name of the doctor</description></item>
        ///   <item><description>AppointmentStatus: Current status of appointments</description></item>
        /// </list>
        /// </returns>

        public async Task<IEnumerable<DoctorAppointmentStatsResponseDto>> GetDoctorAppointmentStats()
        {
            var repositoryResult = await _statsRepository.GetDoctorAppointmentStats();
            Log.Information("Successfully retrieved {Count} doctor appointment stats", repositoryResult.Count());
            var result = repositoryResult.Select(x => new DoctorAppointmentStatsResponseDto
            {
                AppointmentCount = x.Appointment_count,
                DoctorUsername = x.Username,
                AppointmentStatus = x.Status
            }).ToList();
            Log.Information("Mapped {Count} items to response DTO", result.Count);
            return result;


        }
        /// <summary>
        /// Gets doctors grouped by their rating tiers
        /// </summary>
        /// <returns>List of doctors with their average ratings and tier ranking</returns>

        public async Task<IEnumerable<DoctorsRatingTierResponseDto>> GetDoctorsByRatingTier()
        {
            Log.Information("Getting doctors by rating tier");
            var repositoryResult = await _statsRepository.GetDoctorsByRatingTier();
            
            var result = repositoryResult.Select(x => new DoctorsRatingTierResponseDto
            {
                DoctorName = x.Doctor_name,
                 AvgRating = x.Avg_Rating,
                RatingRank = x.Rating_Rank
            }).ToList();
            Log.Information("Returning {DoctorCount} doctors with rating tiers", result.Count);

            return result;


        }
        /// <summary>
        /// Gets patient count grouped by age groups
        /// </summary>
        /// <returns>List of patient counts by age group</returns>
        public async Task<IEnumerable<PatientCountByAgeResponseDto>> GetPatientCountByAgeGroup()
        {
            Log.Information("Getting patient count by age group");
            var repositoryResult = await _statsRepository.GetPatientCountByAgeGroup();

            var result = repositoryResult.Select(x => new PatientCountByAgeResponseDto
            {
                PatientCount = x.patient_count,
                PatientAge = x.patient_age

            }).ToList();
            Log.Information("Returning {PatientCount} age groups with patient counts", result.Count);

            return result;
        }

    }
}
