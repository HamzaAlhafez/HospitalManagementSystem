using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Services.AppointmentManagement;
using HospitalManagementSystem.Services.Interfaces.AppointmentManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers.Appointments
{
    [Route("api/Appointments/Admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    
    public class AppointmentsAdminController : ControllerBase
    {
        readonly private IAppointmentService _appointmentService;
        public AppointmentsAdminController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;

        }
        [HttpGet("GetPagedAppointments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedAppointments([FromQuery] PaginationRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data"));
            }
            var result = await _appointmentService.GetPagedAppointmentsAsync(dto);
            if (result.TotalCount == 0)
               return  NoContent();


            return Ok(result);
        }
        [HttpGet("Appointemnt/{id}",Name ="GetAppointmentByid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<AppointmentResponseDto>> GetAppointmentById(int id)
        {
            if (id < 1)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}"));
            var appointemnt=await _appointmentService.GetAppointmentById(id);
            
            if (!appointemnt.IsSuccess)
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found Appointment"));
            return Ok(appointemnt);





        }
        [HttpGet("doctors/available")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DoctorAvailableResponseDto>>> GetAvailableDoctors()
        {
            var doctors = await _appointmentService.GetAvailableDoctorsAsync();
            if (doctors.Count() == 0)
                return NoContent();


            return Ok(doctors);
        }
        [HttpGet("Patient/available")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<patientAvailableResponseDto>>> GetAvailablePatientAsync()
        {
            var Patient = await _appointmentService.GetAvailablePatientAsync();
            if (Patient.Count() == 0)
                return NoContent();


            return Ok(Patient);
        }
        [HttpPost("CreateAppointment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentResponseDto>>CreateAppointment([FromBody] AppointmentAdminRequestDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            var appointemnt =await  _appointmentService.CreateAppointmentByAdminAsync(dto);
            if(!appointemnt.IsSuccess)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(appointemnt.Message, ModelState));
            
            return CreatedAtRoute("GetAppointmentByid", new {id=appointemnt.AppointmentId},dto);









        }
        [HttpPatch("Update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        
        public async Task<ActionResult<AppointmentResponseDto>> UpdateAppointment(int id, AppointmentAdminRequestDto updateDoctor)
        {
            if (id < 1)

                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}", ModelState));
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            var Appointment = await _appointmentService.UpdateAppointmentByAdminAsync(id, updateDoctor);
            if (Appointment == null)
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found {id}"));
            if (!Appointment.IsSuccess)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(Appointment.Message));
            return Ok(Appointment);










        }
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAppointemt(int id)
        {
            if (id < 1)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}"));
            if (await _appointmentService.DeleteAsync(id))
            {
                return Ok($"Appointment with {id} has benn deleted");

            }

            else
            {
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found Doctor"));

            }

        }

    }
}
