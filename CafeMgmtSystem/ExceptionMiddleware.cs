using System.Net;
using System.Text.Json;

namespace CafeMgmtSystem
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred processing the request");
                await HandleExceptionAsync(context, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new
            {
                Code = context.Response.StatusCode,
                Message = "An unexpected error occurred. Please try again later.",
                Detailed = exception.Message // You can remove this in production for security reasons
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
