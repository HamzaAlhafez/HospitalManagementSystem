using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Services.Interfaces.StatsManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers.Stats
{
    /// <summary>
    /// API controller for managing statistics related operations
    /// </summary>
    [Route("api/Stats")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;
        /// <summary>
        /// Initializes a new instance of the StatsController
        /// </summary>
        /// <param name="statsService">The statistics service</param>
        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;

        }
        /// <summary>
        /// Gets appointment counts grouped by status
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /api/Stats/Appointments/Status
        /// </remarks>
        /// <response code="200">Returns the list of appointment statistics</response>
        /// <response code="204">No appointment statistics found</response>
        /// <response code="500">Internal server error occurred</response>
        /// <returns>List of appointment statistics grouped by status</returns>
        [HttpGet("appointments/by-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<AppointmentStatusCountResponseDto>>> GetAppointmentCountByStatus()
        {
            var result = await _statsService.GetAppointmentCountByStatus();
            if (result.Count() == 0)
                return NoContent();
            return Ok(result);

        }
        /// <summary>
        /// Retrieves appointment statistics grouped by doctor and status
        /// </summary>
        /// <remarks>
        /// ### Sample Request
        /// GET /api/Doctors/Appointments/Status
        /// </remarks>
        /// <response code="200">Returns the list of doctor appointment statistics</response>
        /// <response code="204">No appointment records found</response>
        /// <response code="500">Internal server error occurred</response>

        [HttpGet("doctors/appointments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<DoctorAppointmentStatsResponseDto>>> GetDoctorAppointmentStats()
        {
            var result = await _statsService.GetDoctorAppointmentStats();
            if (result.Count() == 0)
                return NoContent();
            return Ok(result);

        }
        /// <summary>
        /// Retrieves appointment statistics grouped by doctor and status
        /// </summary>
        /// <remarks>
        /// ### Sample Request
        /// GET /api/Doctors/Appointments/Status
        /// </remarks>
        /// <response code="200">Returns the list of doctor appointment statistics</response>
        /// <response code="204">No appointment records found</response>
        /// <response code="500">Internal server error occurred</response>

        [HttpGet("doctors/appointments/current-month")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<DoctorAppointmentStatsResponseDto>>> GetCurrentMonthDoctorAppointments()
        {
            var result = await _statsService.GetCurrentMonthDoctorAppointments();
            if (result.Count() == 0)
                return NoContent();
            return Ok(result);

        }
        /// <summary>
        /// Retrieves doctors grouped by their rating tiers
        /// </summary>
        /// <remarks>
        /// ### Rating Tier Classification
        /// - Tier 1: Average rating 1-2
        /// - Tier 2: Average rating 3-4
        /// - Tier 3: Average rating 4.1-5
        /// 
        /// ### Sample Request
        /// GET /api/Doctors/RatingTiers
        /// </remarks>
        /// <response code="200">Returns list of doctors with their rating tiers</response>
        /// <response code="204">No doctors found with rating data</response>
        /// <response code="500">Internal server error occurred</response>

        [HttpGet("doctors/by-rating")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<DoctorsRatingTierResponseDto>>> GetDoctorsByRatingTier()
        {
            var result = await _statsService.GetDoctorsByRatingTier();
            if (result.Count() == 0)
                return NoContent();
            return Ok(result);

        }
        /// <summary>
        /// Retrieves the count of patients grouped by age categories
        /// </summary>
        /// <remarks>
        /// The age groups are categorized as:
        /// - Under 18
        /// - 18 to 30
        /// - 31 to 50
        /// - Over 50
        /// </remarks>
        /// <returns>
        /// Returns a list of patient counts by age group.
        /// Returns 204 No Content if no data is found.
        /// Returns 500 if there is a server error.
        /// </returns>
        [HttpGet("patients/by-age")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PatientCountByAgeResponseDto>>> GetPatientCountByAgeGroup()
        {
            var result = await _statsService.GetPatientCountByAgeGroup();
            if (result.Count() == 0)
                return NoContent();
            return Ok(result);
        }


    }
}
