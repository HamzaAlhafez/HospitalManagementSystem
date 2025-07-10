using System.Threading.Tasks;
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Services.Interfaces.DoctorManagemment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/Doctors")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorMangementService _DoctorMangementService;
        public DoctorsController(IDoctorMangementService DoctorMangementService)
        {
            _DoctorMangementService = DoctorMangementService;

        }
        [HttpGet("GetPagedDoctors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedDoctors([FromQuery] PaginationRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data"));
            }
            var result = await _DoctorMangementService.GetPagedDoctorsAsync(dto);
            if (result.TotalCount == 0)
                NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found Doctors"));


            return Ok(result);
        }
        [HttpGet("GetDoctorByID/{id}", Name = "GetDoctorID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]



        public async Task<ActionResult<DoctorResponseDto>> GetDoctorByID(int id)
        {
            if (id < 1)

                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}"));
            var doctor = await _DoctorMangementService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse("Not found Doctor"));

            return Ok(doctor);





        }
        [HttpPost("Create")]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreateDoctorResponseDto>> CreateDoctor(CreateDoctorRequestDto doctorRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            var doctor = await _DoctorMangementService.CreateDoctorAsync(doctorRequestDto);
            if (!doctor.IsSuccess)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(doctor.Message));
            return CreatedAtRoute("GetDoctorID", new { id = doctor.id }, doctorRequestDto);


        }
        [HttpPatch("Update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize("Admin,Doctor")]
        public async Task<ActionResult<UpdateDoctorResponseDto>> UpdateDoctor(int id,UpdateDoctorRequestDto updateDoctor)
        {
            if (id < 1)
                
            return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}", ModelState));
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            var doctor = await _DoctorMangementService.UpdateDoctorAsync(id, updateDoctor);
            if (doctor == null)
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found {id}"));
            if (!doctor.IsSuccess)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(doctor.Message));
            return Ok(new { doctor.Message, doctor.IsSuccess });










        }
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteDoctor(int id)
        {
            if (id < 1)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}"));
            if (await _DoctorMangementService.DeleteDoctorAsync(id))
            {
                return Ok($"Doctor with {id} has benn deleted");

            }

            else
            {
                return  NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found Doctor"));

            }
           
        }
               
          







        





















    }
}
