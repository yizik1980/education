using boarding_school_api.Models;

namespace boarding_school_api.Infrastructure
{
    public interface IStudentsRepository
    {
        Task DeleteStudent(int studentId);

        Task<IQueryable<Student>> GetAllStudentsAsync();

        Task<IQueryable<Student>> GetStudentsByPlaceIdAsync(int placeId);

        Task<IEnumerable<Student>> InsertNewStudent(Student student);

        Task<Student> UpdateStudent(Student student);
    }
}
