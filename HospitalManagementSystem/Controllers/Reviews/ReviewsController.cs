using System.Security.Claims;
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Services.Interfaces.ReviewManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers.Reviews
{
    [Route("api/Reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService; 
        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;

        }
        [HttpGet("{id}", Name = "GetReviewByid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Patient")]

        public async Task<ActionResult<ReviewinfoResponseDto?>> GetByIdAsync(int id)
        {
            if (id < 1)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}"));
            var Review = await _reviewService.GetByIdAsync(id);

            if (Review==null)
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found review"));
            return Ok(Review);





        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<ReviewResponseDto>> AddReview(int patientId, CreateReviewRequestDto reviewDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid user identity"));
            var review = await _reviewService.AddPatientReviewAsync(currentUserId, reviewDto);
            if (!review.IsSuccess)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(review.Message));

            return CreatedAtRoute("GetReviewByid", new { id = review.reviewid }, review);









        }
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Patient")]

        public async Task<ActionResult<AppointmentResponseDto>> UpdateAppointment(int id, UpdateReviewRequestDto requestDto)
        {
            if (id < 1)

                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}", ModelState));
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));
            var review = await _reviewService.UpdatePatientReviewAsync(id, requestDto);
            if (review == null)
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found {id}"));
            if (!review.IsSuccess)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse(review.Message));
            return Ok(new {review.Message});
        }
        [HttpGet("patient/MyReviews")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<IEnumerable<AllReviewsResponseDto>>> GetPatientReviews()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid user identity"));
            var PatientReview = await _reviewService.GetPatientReviewsAsync(currentUserId);
            if (PatientReview.Count() == 0)
                return NoContent();


            return Ok(PatientReview);
        }
        [HttpGet("doctor/{doctorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<ActionResult<IEnumerable<AllReviewsResponseDto>>> GetDoctorReviews(int doctorId)
        {

            var doctorReviews = await _reviewService.GetDoctorReviewsAsync(doctorId);
            if (doctorReviews.Count() == 0)
                return NoContent();


            return Ok(doctorReviews);
        }




        [HttpGet("doctor/{doctorId}/average-rating")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Patient,Doctor")]


        public async Task<ActionResult<float>> GetDoctorAverageRating(int doctorId)
        {

            var doctorAvg = await _reviewService.GetDoctorAverageRatingAsync(doctorId);
            if (doctorAvg == 0)
                return NotFound();


            return Ok(doctorAvg);
        }
        [HttpGet("filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<IEnumerable<ReviewFilterResponsDto>>> GetFilteredReviews(
    [FromQuery] int? doctorId = null,
    [FromQuery] int? patientId = null,
    [FromQuery] int? minRating = null,
    [FromQuery] int? maxRating = null,
    [FromQuery] DateTime? datetime = null,
    [FromQuery] string sortBy = "newest")
        {
            var filter = new ReviewFilterRequestDto
            {
                DoctorId = doctorId,
                PatientId = patientId,
                MinRating = minRating,
                MaxRating = maxRating,
                Datetime = datetime,
                SortBy = sortBy
            };

            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse("Invalid request data", ModelState));

            var ReviewFilter = await _reviewService.GetFilteredReviewsAsync(filter);

            if (ReviewFilter.Count() == 0)
                return NoContent();

            return Ok(ReviewFilter);
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseHandler.ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteReview(int id)
        {
            if (id < 1)
                return BadRequest(new ApiErrorResponseHandler.ApiErrorResponse($"Not Accpeted {id}"));
            if (await _reviewService.DeleteAsync(id))
            {
                return Ok($"Doctor with {id} has benn deleted");

            }

            else
            {
                return NotFound(new ApiErrorResponseHandler.ApiErrorResponse($"Not Found Doctor"));

            }

        }

















    }
}
