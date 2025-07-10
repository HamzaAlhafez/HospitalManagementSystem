using System.Security.Claims;
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Services.Interfaces.AppointmentManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers.Appointments
{
    [Route("api/Appointments/Doctor")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    
    public class AppointmentsDoctorController : ControllerBase
    {

        private readonly IAppointmentService _appointmentService;
        public AppointmentsDoctorController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;

        }
        
        [HttpPost("CreateAppointment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]

        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentResponseDto>> CreateAppointment([FromBody] AppointmentDoctorRequestDto dto)
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


            var appointemnt = await _appointmentService.CreateAppointmentByDoctorAsync(dto, currentUserId, UsernameClaim.Value);
            if (!appointemnt.IsSuccess)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(appointemnt.Message, ModelState));

            return CreatedAtRoute("GetAppointmentByid", new { id = appointemnt.AppointmentId }, dto);









        }
        [HttpGet("GetAllAppointmentsDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto?>>> GetAppointmentsDoctorByIdAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized(new ApiErrorResponseHandler.ApiErrorResponse("User not authenticated"));
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid user identity"));
            var doctorAppointemnt = await _appointmentService.GetAppointmentDoctorByIdAsync(currentUserId);
            if (doctorAppointemnt == null)
                return NoContent();
            return Ok(doctorAppointemnt);
        }
        [HttpPatch("{appointmentId}/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Confirm(int appointmentId)
        {
            var result = await _appointmentService.ConfirmAppointment(appointmentId);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(result.Message));
        }

        [HttpPatch("{appointmentId}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(int appointmentId, [FromBody] CancelRequestDto request)
        {
            var result = await _appointmentService.CancelAppointment(appointmentId, request.CancellationReason);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(result.Message));
        }

        [HttpPatch("{appointmentId}/complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Doctor,Patient")]
        public async Task<IActionResult> Complete(int appointmentId)
        {
            var result = await _appointmentService.CompleteAppointment(appointmentId);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(result.Message));
        }
        [HttpPatch("Update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentResponseDto>> UpdateAppointment(int id, AppointmentDoctorRequestDto dto)
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

            var Appointment = await _appointmentService.UpdateAppointmentByDoctorAsync(id,dto,currentUserId);
            if (Appointment == null)
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found {id}"));
            if (!Appointment.IsSuccess)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(Appointment.Message));
            return Ok(Appointment);
        }




















        




    }
}
