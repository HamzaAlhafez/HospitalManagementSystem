using HospitalManagementSystem.DTOs.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using HospitalManagementSystem.Services.Interfaces.Auth;
using HospitalManagementSystem.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using HospitalManagementSystem.Filters.Cookie;
using System.Security.Claims;
using HospitalManagementSystem.ApiErrorResponseHandler;





namespace HospitalManagementSystem.Controllers
{
    [Route("api/Auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _Authservice;
        public AuthController(IAuthService Authservice) 
        {
            _Authservice = Authservice;
        }
        
       
        
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SetRefreshTokenCookie]

        public async Task<ActionResult> Login(LoginRequestDto requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            var result = await _Authservice.LoginAsync(requestDto);
            if (!result.IsAuthenticated)
                return BadRequest(new { result.IsAuthenticated, result.Message});
            return Ok(result);
            
          



            //return Ok(new { result.IsAuthenticated, result.Message, result.Token, result.Roles, result.ExpiresOn,result.RefreshToken,result.RefreshTokenExpiration });




        }
        [HttpPost("RefreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SetRefreshTokenCookie]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _Authservice.RefreshTokenAsync(refreshToken);

            if (!result.IsAuthenticated)
                return BadRequest(new {result.Message,result.IsAuthenticated});

            
            

            return Ok(new { result.IsAuthenticated, result.Message, result.Token, result.Roles, result.ExpiresOn });
        }
        [HttpPost("RevokeToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RevokeToken(string RevokeToken)
        {
            var token = RevokeToken?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Token is required!"));



            var result = await _Authservice.RevokeTokenAsync(token);

            if (!result)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Token is invalid!"));



            return Ok("RevokeToken successfully!");
        }
        
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // 1. Get refresh token from cookie
            var refreshToken = Request.Cookies["refreshToken"];

            // 2. Check if token exists
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Refresh token is required!"));
            

            // 3. Revoke the token
            var result = await _Authservice.RevokeTokenAsync(refreshToken);

            if (!result)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Token is invalid!"));
            


            // 4. Delete cookie from browser
            Response.Cookies.Delete("refreshToken");

            // 5. Return success
            return Ok("Logged out successfully!");
        }
        
        





















    }
}
