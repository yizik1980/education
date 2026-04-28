using System.Net;
using System.Text.Json;

namespace boarding_school_api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex);
                
                // Here we would call the logging microservice
                await ReportToLoggingService(ex.Message);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(new
            {
                error = "A critical incident occurred. Our team has been alerted.",
                details = exception.Message
            });

            return context.Response.WriteAsync(result);
        }

        private async Task ReportToLoggingService(string message)
        {
            try
            {
                using var client = new HttpClient();
                var loggingUrl = _configuration["LoggingServiceUrl"] ?? "http://localhost:3001/logs";
                var logData = new { level = "CRITICAL", message = message, timestamp = DateTime.UtcNow };
                await client.PostAsJsonAsync(loggingUrl, logData);
            }
            catch (Exception)
            {
                // If logging service is down, we don't want to crash the main API
            }
        }
    }
}
