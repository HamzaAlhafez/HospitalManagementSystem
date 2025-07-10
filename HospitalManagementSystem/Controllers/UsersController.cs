using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.Services.UserManagement;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HospitalManagementSystem.ApiErrorResponseHandler;
using HospitalManagementSystem.Services.Interfaces.Auth;
using HospitalManagementSystem.Services.Interfaces.UserManagement;



namespace HospitalManagementSystem.Controllers
{
    [Route("api/UsersManagement")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementService _UserManagementService;
        public UsersController(IUserManagementService UserManagementService)
        {
            _UserManagementService = UserManagementService;
        }
        [HttpPatch("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        [Authorize]




        public async Task<ActionResult> ChangePassword(ChangePasswordRequestDto changePasswordRequestDto)
        {
            // 1. Authentication Check
            if (!User.Identity.IsAuthenticated)
                return Unauthorized(new ApiErrorResponseHandler.ApiErrorResponse("User not authenticated"));
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            // 3. User ID Extraction
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid user identity"));
            // 4. Service Call
            var result = await _UserManagementService.ChangePasswordAsync(currentUserId, changePasswordRequestDto);
            // 5. Handle Service Result
            if (!result.IsSuccess)
                return BadRequest(new { result.IsSuccess, result.Message });
            return Ok(new { result.IsSuccess, result.Message });




        }
        [HttpPatch("Deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]




        public async Task<ActionResult> DeactivateUser()
        {
            // 1. Authentication Check
            if (!User.Identity.IsAuthenticated)
                return Unauthorized(new ApiErrorResponseHandler.ApiErrorResponse("User not authenticated"));
           
            // 3. User ID Extraction
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid user identity"));
            // 4. Service Call
            var result = await _UserManagementService.DeactivateUserAsync(currentUserId);
            // 5. Handle Service Result
            if (!result.IsSuccess)
                return BadRequest(new { result.IsSuccess, result.Message });
            return Ok(new { result.IsSuccess, result.Message });

        }




    }
}
