using HospitalManagementSystem.ApiErrorResponse;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HospitalManagementSystem.ApiErrorResponseHandler
{
    public class ApiErrorResponse : ApiResponse
    {
        public IDictionary<string, string[]> Errors { get; set; }

        public ApiErrorResponse(string message, ModelStateDictionary modelState = null)
        {
            Success = false;
            Message = message;
            Errors = modelState?.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
        }

    }
}
