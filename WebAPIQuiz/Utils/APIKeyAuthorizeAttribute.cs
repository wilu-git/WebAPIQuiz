using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIQuiz.Models;

namespace WebAPIQuiz.Utils
{
    public class APIKeyAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<APIKeyAuthorizeAttribute>>();
            var config = context.HttpContext.RequestServices.GetService<IConfiguration>();

            if (!context.HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var extractedKey))
            {
                logger.LogWarning("Missing API Key attempt");
                context.Result = new UnauthorizedObjectResult("API Key missing");
                return;
            }

            var apiKeys = config?
                .GetSection("Security:ApiKeys")
                .Get<List<ApiKeyConfig>>();

            var validKey = apiKeys?.FirstOrDefault(k => k.Key == extractedKey.ToString());

            if (validKey == null)
            {
                logger.LogWarning("Invalid API Key attempt: {key}", extractedKey);
                context.Result = new UnauthorizedObjectResult("Invalid API Key");
                return;
            }

            if (validKey.Expires < DateTime.UtcNow)
            {
                logger.LogWarning("Expired API Key attempt: {key}", extractedKey);
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
