using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyFirestoreApi.Services;

namespace MyFirestoreApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class VerifyCorpAttribute : Attribute, IAsyncActionFilter
    {
        private CorpService _cropService;

        public VerifyCorpAttribute()
        {
            _cropService = new CorpService();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Retrieve the API key from the headers
            if (!context.HttpContext.Request.Headers.TryGetValue("x-api-key", out var apiKeyValues))
            {
                context.Result = new UnauthorizedObjectResult("API key is missing."); // HTTP 401 Unauthorized
                return;
            }

            var apiKey = apiKeyValues.ToString();

            if (string.IsNullOrEmpty(apiKey))
            {
                context.Result = new BadRequestObjectResult("API key is empty."); // HTTP 400 Bad Request
                return;
            } 

            // Check the length of the API key
            if (apiKey.Length != 100 || !apiKey.All(char.IsLetterOrDigit))
            {
                context.Result = new BadRequestObjectResult($"Invalid API key length. It must be exactly 64 characters. Your header length is {apiKey.Length}"); // HTTP 400 Bad Request
                return;
            } 

            {
                var corpCollectionIdDict = await _cropService.GetCorpIdByHeaderApiKeyAsync(apiKey);

                if (corpCollectionIdDict.TryGetValue("corpCollectionId", out var corpCollectionIdObj))
                {
                    if (corpCollectionIdObj is string corpCollectionId)
                    {
                        
                        context.HttpContext.Items["corpCollectionId"] = corpCollectionId;
                        await next();
                    }
                    else
                    {
                        context.Result = new BadRequestObjectResult("Invalid corpId format."); // HTTP 400 Bad Request
                    }
                }
                else
                {
                    context.Result = new NotFoundObjectResult("API key is valid but corpId not found."); // HTTP 404 Not Found
                }
            }
        }
    }
}
