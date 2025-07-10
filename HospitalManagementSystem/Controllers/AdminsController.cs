using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Services.Interfaces.AdminManagement;
using HospitalManagementSystem.Services.Interfaces.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/AdminsManagement")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminManagementService _adminManagementService;
        public AdminsController(IAdminManagementService adminManagementService)
        {
            _adminManagementService =adminManagementService;
        }
        [HttpGet("GetPagedAdmins")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedAdmins([FromQuery] PaginationRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data"));
            }
            var result = await _adminManagementService.GetPagedAdminsAsync(dto);
            if (result.TotalCount == 0)
                NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found Admins"));


            return Ok(result);
        }
        [HttpGet("GetAdminByID/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]



        public async Task<ActionResult<AdminResponseDto>> GetAdminByID(int id)
        {
            if (id < 1)

                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}"));
            var Admin = await _adminManagementService.GetAdminByIdAsync(id);
            if (Admin == null)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Not found Admin"));

            return Ok(Admin);





        }
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> RegisterAdmin(RegisterAdminDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            var result = await _adminManagementService.RegisterAdminAsync(registerDto);
            if (!result.IsSuccess)
                return BadRequest(new { result.IsSuccess, result.Message });
            return Ok(new { result.IsSuccess, result.Message });


        }
        [HttpPatch("Update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UpdateAdminResponseDto>> UpdateAdmin(int id, UpdateAdminRequestDto updateAdmin)
        {
            if (id < 1)

                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}", ModelState));
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            var Admin = await _adminManagementService.UpdateAdminAsync(id, updateAdmin);
            if (Admin == null)
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found {id}"));
            if (!Admin.IsSuccess)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(Admin.Message));
            return Ok(new { Admin.Message, Admin.IsSuccess });










        }
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAdmin(int id)
        {
            if (id < 1)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}"));
            if (await _adminManagementService.DeleteAdminAsync(id))
            {
                return Ok($"Admin with {id} has benn deleted");

            }

            else
            {
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found Doctor"));

            }

        }



    }
}
