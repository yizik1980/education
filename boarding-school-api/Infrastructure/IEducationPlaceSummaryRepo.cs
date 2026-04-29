namespace boarding_school_api.Infrastructure
{
    public interface IEducationPlaceSummaryRepo
    {
        Task<IEnumerable<Models.EducationPlaceSummary>> GetSummaryAsync(string? city = null, int minStudents = 0);
    }
}
