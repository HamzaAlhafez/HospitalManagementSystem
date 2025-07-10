using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HospitalManagementSystem.Filters.Cookie
{
    public class SetRefreshTokenCookieAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is not OkObjectResult result || result.Value == null)
                return;

            
            var token = result.Value.GetType().GetProperty("RefreshToken")?.GetValue(result.Value) as string;
            var expires = result.Value.GetType().GetProperty("RefreshTokenExpiration")?.GetValue(result.Value) as DateTime?;

            if (token == null || !expires.HasValue)
                return;

            
            var response = context.HttpContext.Response;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires.Value.ToLocalTime(),
                Path = "/api/Auth/",

            };

            response.Cookies.Append("refreshToken", token, cookieOptions);
        }


        
    }
}
