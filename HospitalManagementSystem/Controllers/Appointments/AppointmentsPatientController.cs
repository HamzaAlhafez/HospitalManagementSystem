using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Services.Interfaces.AppointmentManagement;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagementSystem.Controllers.Appointments
{
    [Route("api/Appointments/Patient")]
    [ApiController]
    [Authorize(Roles = "Patient")]

    
    public class AppointmentsPatientController : ControllerBase
    {


        private readonly IAppointmentService _appointmentService;
        public AppointmentsPatientController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;

        }

        [HttpPost("CreateAppointment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]

        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentResponseDto>> CreateAppointment([FromBody] AppointmentPatientRequestDto dto)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized(new ApiErrorResponseHandler.ApiErrorResponse("User not authenticated"));
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid user identity"));
            var UsernameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim == null)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid user identity"));


            var appointemnt = await _appointmentService.CreateAppointmentByPatientAsync(dto, currentUserId, UsernameClaim.Value);
            if (!appointemnt.IsSuccess)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(appointemnt.Message, ModelState));

            return CreatedAtRoute("GetAppointmentByid", new { id = appointemnt.AppointmentId }, dto);









        }
        [HttpGet("GetAllAppointmentsPatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto?>>> GetAppointmentsPatientByIdAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized(new ApiErrorResponseHandler.ApiErrorResponse("User not authenticated"));
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid user identity"));
            var patientAppointemnt = await _appointmentService.GetAppointmentPatientByIdAsync(currentUserId);
            if (patientAppointemnt == null)
                return NoContent();
            return Ok(patientAppointemnt);
        }

        [HttpPatch("Update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentResponseDto>> UpdateAppointment(int id, AppointmentPatientRequestDto dto)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized(new ApiErrorResponseHandler.ApiErrorResponse("User not authenticated"));
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid user identity"));
            if (id < 1)

                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}", ModelState));
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));

            var Appointment = await _appointmentService.UpdateAppointmentByPatientAsync(id, dto, currentUserId);
            if (Appointment == null)
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found {id}"));
            if (!Appointment.IsSuccess)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(Appointment.Message));
            return Ok(Appointment);
        }

    }
}
