using System.Text.Json;
using boarding_school_api.Services;

namespace boarding_school_api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly ILoggingService _loggingService;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, ILoggingService loggingService)
        {
            _next = next;
            _logger = logger;
            _loggingService = loggingService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);

                var statusCode = GetStatusCode(ex);
                var level = statusCode >= 500 ? "ERROR" : "WARN";
                var logMessage = $"[{context.Request.Method} {context.Request.Path}] {ex.GetType().Name}: {ex.Message}";

                await _loggingService.LogAsync(level, logMessage);
                await WriteResponseAsync(context, ex, statusCode);
            }
        }

        private static int GetStatusCode(Exception exception) => exception switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            InvalidOperationException => StatusCodes.Status409Conflict,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        private static async Task WriteResponseAsync(HttpContext context, Exception exception, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var error = statusCode >= 500
                ? "אירעה שגיאה פנימית. הצוות שלנו קיבל התראה."
                : exception.Message;

            var result = JsonSerializer.Serialize(new { error });
            await context.Response.WriteAsync(result);
        }
    }
}
