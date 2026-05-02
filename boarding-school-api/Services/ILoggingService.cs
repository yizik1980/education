namespace boarding_school_api.Services
{
    public interface ILoggingService
    {
        Task LogAsync(string level, string message);
    }
}
