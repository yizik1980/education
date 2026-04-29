using boarding_school_api.Data;
using boarding_school_api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace boarding_school_api.Infrastructure
{
    public class BoardingSchoolQuery : IBoardingSchoolQuery
    {
        private readonly BoardingSchoolContext _repository;
        public BoardingSchoolQuery(BoardingSchoolContext repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<ActiveStudentsByPlace>> GetAllBoardingSchoolsSPAsync()
        {
            return await _repository.ActiveStudentsByPlace
                .FromSqlRaw("SELECT EducationPlaceId,PlaceName,City,ActiveStudentCount,AvrageAges FROM vw_ActiveStudentsByPlace")
                .AsNoTracking().ToListAsync();
        }
    }
}
