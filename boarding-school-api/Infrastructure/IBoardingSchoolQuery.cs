namespace boarding_school_api.Infrastructure
{
    public interface IBoardingSchoolQuery
    {
        Task<IEnumerable<Models.ActiveStudentsByPlace>> GetAllBoardingSchoolsSPAsync();
    }
}
