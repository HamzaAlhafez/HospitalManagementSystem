using HospitalManagementSystem.DTOs.databse;
using HospitalManagementSystem.DTOs.Internal;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Repositories.Interfaces.StatsManagement;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace HospitalManagementSystem.Repositories.StatsManagement
{
    public class StatsRepository : IStatsRepository
    {
        private readonly ApplicationDbContext _context;
        public StatsRepository(ApplicationDbContext context)
        {
            _context = context;
            

        }
        /// <summary>
        /// Retrieves appointment statistics grouped by status from the database using a stored procedure.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="Appointment"/> objects containing counts grouped by status.
        /// Returns an empty list if no data is found or an error occurs.
        /// </returns>
       
        public async Task<IEnumerable<AppointmentStatusCountResultInternalDto>> GetAppointmentCountByStatus()
        {
            const string methodName = nameof(GetAppointmentCountByStatus);

            try
            {
                Log.Information("Executing {MethodName} to fetch appointment statistics", methodName);
                var result = await _context.Database
          .SqlQuery<AppointmentStatusCountResultInternalDto>($"EXEC Sp_GetAppointmentCountByStatus")
          .ToListAsync();

                Log.Information("{MethodName} successfully retrieved {AppointmentCount} records",
                              methodName, result.Count);
                if (result.Count == 0)
                {
                    Log.Warning(
                        "{MethodName} returned empty results",
                        methodName
                    );
                }

                return result;


            }
            catch (Exception ex)
            {
                Log.Error(ex, "{MethodName} failed: {ErrorMessage}",
                         methodName, ex.Message);
                return Enumerable.Empty<AppointmentStatusCountResultInternalDto>();


            }
        }
        /// <summary>
        /// Retrieves appointment statistics Current Month  grouped by doctor and status.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="DoctorAppointmentStatsResultInternalDto"/> containing:
        /// - AppointmentCount: The number of appointments
        /// - Username: The doctor's username
        /// - Status: The appointment status
        /// </returns>

        public async Task<IEnumerable<DoctorAppointmentStatsResultInternalDto>> GetCurrentMonthDoctorAppointments()
        {
            const string methodName = nameof(GetCurrentMonthDoctorAppointments);

            try
            {
                Log.Information(
           "Starting {MethodName} - Executing stored procedure ",
           methodName

       );
                var result = await _context.Database
          .SqlQuery<DoctorAppointmentStatsResultInternalDto>($"EXEC Sp_GetCurrentMonthDoctorAppointments")
          .ToListAsync();

                Log.Information(
          "{MethodName} completed successfully - Retrieved {RecordCount} doctor appointment In Current Month  records",
          methodName,
          result.Count
      );
                if (result.Count == 0)
                {
                    Log.Warning(
                        "{MethodName} returned empty results - No doctor appointment statistics found In Current Month",
                        methodName
                    );
                }

                return result;


            }
            catch (Exception ex)
            {
                Log.Error(ex, "{MethodName} failed: {ErrorMessage}",
                         methodName, ex.Message);
                return Enumerable.Empty<DoctorAppointmentStatsResultInternalDto>();


            }

        }

        /// <summary>
        /// Retrieves appointment statistics grouped by doctor and status.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="DoctorAppointmentStatsResultInternalDto"/> containing:
        /// - AppointmentCount: The number of appointments
        /// - Username: The doctor's username
        /// - Status: The appointment status
        /// </returns>

        public async Task<IEnumerable<DoctorAppointmentStatsResultInternalDto>> GetDoctorAppointmentStats()
        {
            const string methodName = nameof(GetDoctorAppointmentStats);

            try
            {
                Log.Information(
           "Starting {MethodName} - Executing stored procedure ",
           methodName
           
       );
                var result = await _context.Database
          .SqlQuery<DoctorAppointmentStatsResultInternalDto>($"EXEC Sp_GetDoctorAppointmentStats")
          .ToListAsync();

                Log.Information(
          "{MethodName} completed successfully - Retrieved {RecordCount} doctor appointment records",
          methodName,
          result.Count
      );
                if (result.Count == 0)
                {
                    Log.Warning(
                        "{MethodName} returned empty results - No doctor appointment statistics found",
                        methodName
                    );
                }

                return result;


            }
            catch (Exception ex)
            {
                Log.Error(ex, "{MethodName} failed: {ErrorMessage}",
                         methodName, ex.Message);
                return Enumerable.Empty<DoctorAppointmentStatsResultInternalDto>();


            }

        }
        /// <summary>
        /// Retrieves doctors grouped by their average rating tiers
        /// </summary>
        /// <returns>
        /// List of doctors with their average ratings and tier ranking
        /// </returns>
        /// A collection of <see cref="DoctorsByRatingTierResultInternalDto"/> containing:
        /// - Doctor_name
        /// - Avg_Rating
        /// - Rating_Rank 
        /// </returns> 


        public async Task<IEnumerable<DoctorsByRatingTierResultInternalDto>> GetDoctorsByRatingTier()
        {
            const string methodName = nameof(GetDoctorsByRatingTier);
            try
            {
                Log.Information(
           "Starting {MethodName} - Executing stored procedure ",
           methodName

       );
                var result = await _context.Database
          .SqlQuery<DoctorsByRatingTierResultInternalDto>($"EXEC Sp_GetDoctorsByRatingTier")
          .ToListAsync();

                if (result.Count == 0)
                {
                    Log.Warning("{MethodName} returned empty results - No doctors found with ratings",
                               methodName);
                }
                else
                {
                    Log.Information("{MethodName} completed - Found {DoctorCount} doctors",
                                  methodName, result.Count);
                }



                return result;


            }
            catch (Exception ex)
            {
                Log.Error(ex, "{MethodName} failed: {ErrorMessage}",
                         methodName, ex.Message);
                return Enumerable.Empty<DoctorsByRatingTierResultInternalDto>();


            }


        }
        /// <summary>
        /// Retrieves patient counts categorized by age groups
        /// </summary>
        /// <returns>
        /// A collection of <see cref="PatientCountByAgeGroupResultInternalDto"/> containing:
        /// <list type="bullet">
        ///   <item><description>patient_count: Number of patients in age group</description></item>
        ///   <item><description>patient_age: Age group category (Under 18, 18-30, etc.)</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// Age groups are calculated as:
        /// <list type="bullet">
        ///   <item><description>Under 18</description></item>
        ///   <item><description>18-30</description></item>
        ///   <item><description>31-50</description></item>
        ///   <item><description>Over 50</description></item>
        /// </list>
        /// </remarks>

        public async Task<IEnumerable<PatientCountByAgeGroupResultInternalDto>> GetPatientCountByAgeGroup()
        {
            const string methodName = nameof(GetPatientCountByAgeGroup);
            try
            {
                Log.Information(
           "Starting {MethodName} - Executing stored procedure ",
           methodName

       );
                var result = await _context.Database
          .SqlQuery<PatientCountByAgeGroupResultInternalDto>($"EXEC Sp_GetPatientCountByAgeGroup")
          .ToListAsync();
                if (result.Count == 0)
                {
                    Log.Warning("{MethodName} returned empty results - No doctors found with ratings",
                               methodName);
                }
                else
                {
                    Log.Information("{MethodName} completed - Found {DoctorCount} doctors",
                                  methodName, result.Count);
                }





                return result;


            }
            catch (Exception ex)
            {
                Log.Error(ex, "{MethodName} failed: {ErrorMessage}",
                         methodName, ex.Message);
                return Enumerable.Empty<PatientCountByAgeGroupResultInternalDto>();


            }

        }
    }
}
