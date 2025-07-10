using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Services.AdminManagement;
using HospitalManagementSystem.Services.Interfaces.AdminManagement;
using HospitalManagementSystem.Services.Interfaces.PatientManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/PatientsManagement")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientManagementService _patientManagementService;
        public PatientsController(IPatientManagementService patientManagementService)
        {
            _patientManagementService = patientManagementService;
        }
        [HttpGet("GetPagedpatients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedpatients([FromQuery] PaginationRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data"));
            }
            var result = await _patientManagementService.GetPagedPatientsAsync(dto);
            if (result.TotalCount == 0)
                NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found Patients"));


            return Ok(result);
        }
        [HttpGet("GetPatientByID/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]



        public async Task<ActionResult<PatientResponseDto>> GetPatientByID(int id)
        {
            if (id < 1)

                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}"));
            var patient = await _patientManagementService.GetPatientByIdAsync(id);
            if (patient == null)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Not found patient"));

            return Ok(patient);





        }
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> CreatePatient(CreatePatientDto createPatient)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            var result = await _patientManagementService.CreatePatientAsync(createPatient);
            if (!result.IsSuccess)
                return BadRequest(new { result.IsSuccess, result.Message });
            return Ok(new { result.IsSuccess, result.Message });


        }
        [HttpPatch("Update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize("Admin,Patient")]
        public async Task<ActionResult<UpdatePatientResponseDto>> UpdatePatient(int id,UpdatePatientRequestDto updatePatient)
        {
            if (id < 1)

                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}", ModelState));
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            var patient = await _patientManagementService.UpdatePatientAsync(id,updatePatient);
            if (patient == null)
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found {id}"));
            if (!patient.IsSuccess)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(patient.Message));
            return Ok(new { patient.Message, patient.IsSuccess });










        }
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> DeletePatient(int id)
        {
            if (id < 1)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}"));
            if (await _patientManagementService.DeletePatientAsync(id))
            {
                return Ok($"patient with {id} has benn deleted");

            }

            else
            {
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found Doctor"));

            }

        }



    }
}
