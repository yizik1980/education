using System.Net.Http.Json;

namespace boarding_school_api.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _loggingUrl;

        public LoggingService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _loggingUrl = configuration["LoggingServiceUrl"] ?? "http://localhost:3001/logs";
        }

        public async Task LogAsync(string level, string message)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("logging");
                await client.PostAsJsonAsync(_loggingUrl, new
                {
                    level,
                    message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch
            {
                // Logging must never throw or disrupt the main API
            }
        }
    }
}
