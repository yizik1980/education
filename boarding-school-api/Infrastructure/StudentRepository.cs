using boarding_school_api.Data;
using boarding_school_api.Models;
using Microsoft.EntityFrameworkCore;

namespace boarding_school_api.Infrastructure
{
    public class StudentRepository : IStudentsRepository
    {
        private readonly StudentContext _studentContext;

        public StudentRepository(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        public async Task DeleteStudent(int studentId)
        {
            var student = await _studentContext.Students.FindAsync(studentId)
                ?? throw new KeyNotFoundException($"תלמיד עם מזהה {studentId} לא נמצא.");

            _studentContext.Students.Remove(student);
            await _studentContext.SaveChangesAsync();
        }

        public async Task<IQueryable<Student>> GetAllStudentsAsync() =>
            await Task.FromResult(_studentContext.Students.AsQueryable().AsNoTracking());

        public async Task<IQueryable<Student>> GetStudentsByPlaceIdAsync(int placeId) =>
            await Task.FromResult(_studentContext.Students
                .Where(s => s.EducationPlaceId == placeId)
                .AsQueryable()
                .AsNoTracking());

        public async Task<IEnumerable<Student>> InsertNewStudent(Student student)
        {
            if (await _studentContext.Students.AnyAsync(s => s.NationalId == student.NationalId))
                throw new InvalidOperationException($"תלמיד עם תעודת זהות {student.NationalId} כבר קיים במערכת.");

            _studentContext.Students.Add(student);
            await _studentContext.SaveChangesAsync();
            return new List<Student> { student };
        }

        public async Task<Student> UpdateStudent(Student student)
        {
            var existing = await _studentContext.Students.FindAsync(student.StudentId)
                ?? throw new KeyNotFoundException($"תלמיד עם מזהה {student.StudentId} לא נמצא.");

            if (await _studentContext.Students.AnyAsync(s => s.NationalId == student.NationalId && s.StudentId != student.StudentId))
                throw new InvalidOperationException($"תלמיד אחר עם תעודת זהות {student.NationalId} כבר קיים במערכת.");

            existing.FullName = student.FullName;
            existing.NationalId = student.NationalId;
            existing.Age = student.Age;
            existing.EducationPlaceId = student.EducationPlaceId;
            existing.StatusId = student.StatusId;

            await _studentContext.SaveChangesAsync();
            return existing;
        }
    }
}
