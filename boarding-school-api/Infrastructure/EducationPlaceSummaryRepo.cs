using boarding_school_api.Data;
using boarding_school_api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace boarding_school_api.Infrastructure
{
    public class EducationPlaceSummaryRepo : IEducationPlaceSummaryRepo
    {
        private readonly EducationPlaceSummaryContext _repository;
        public EducationPlaceSummaryRepo(EducationPlaceSummaryContext repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<EducationPlaceSummary>> GetSummaryAsync(string? city = null, int minStudents = 0)
        {
            var cityParam = new SqlParameter("@City", (object?)city ?? DBNull.Value);
            var minParam = new SqlParameter("@MinStudents", minStudents);

            return await _repository.EducationPlaceSummaryDataSet
                .FromSqlRaw(
                    "EXEC usp_GetEducationPlaceSummary @City, @MinStudents",
                    cityParam,
                    minParam
                )
                .ToListAsync();
        }
    }
}
