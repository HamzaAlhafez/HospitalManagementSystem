using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.Services.Interfaces.MailingManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers.Mailing
{
    [Route("api/Mailing")]
    [ApiController]
    public class MailingController : ControllerBase
    {
        private readonly IMailingService _mailingService;

        public MailingController(IMailingService mailingService)
        {
            _mailingService = mailingService;
        }
        [HttpPost("send")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendMail([FromForm] MailRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            await _mailingService.SendEmailAsync(dto.ToEmail, dto.Subject, dto.Body,dto.Attachments);
            return Ok();
        }


    }
}
