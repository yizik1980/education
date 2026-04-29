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
            var student = _studentContext.Students.Where(s => s.StudentId == studentId).FirstOrDefault();
            if (student != null)
            {
                _studentContext.Students.Remove(student);
                await _studentContext.SaveChangesAsync();
            }
        }

        public async Task<IQueryable<Student>> GetAllStudentsAsync()
        {
            return await Task.FromResult(_studentContext.Students.AsQueryable().AsNoTracking());
        }

        public async Task<IQueryable<Student>> GetStudentsByPlaceIdAsync(int PlaceId)
        {
            return await Task.FromResult(_studentContext.Students.Where(s => s.EducationPlaceId == PlaceId).AsQueryable());
        }

        public async Task<IEnumerable<Student>> InsertNewStudent(Student student)
        {
            validateStudent(student);
            _studentContext.Students.Add(student);
            await _studentContext.SaveChangesAsync();
            return await Task.FromResult<IEnumerable<Student>>(new List<Student> { student });
        }

        public Task<Student> UpdateStudent(Student student)
        {
            validateStudent(student);
            var existingStudent = _studentContext.Students.Where(s => s.StudentId == student.StudentId).FirstOrDefault();
            if (existingStudent != null)
            {
                existingStudent.FullName = student.FullName;
                existingStudent.NationalId = student.NationalId;
                existingStudent.Age = student.Age;
                existingStudent.EducationPlaceId = student.EducationPlaceId;
                existingStudent.StatusId = student.StatusId;
                _studentContext.Update(existingStudent);
                return Task.FromResult<Student>(existingStudent);
            }
            throw new ArgumentException($"Student with ID {student.StudentId} not found.");
        }
        private void validateStudent(Student student)
        {
            ArgumentNullException.ThrowIfNull(student);

            if (string.IsNullOrWhiteSpace(student.FullName))
                throw new ArgumentException("FullName is required.");

            if (string.IsNullOrWhiteSpace(student.NationalId))
                throw new ArgumentException("NationalId is required.");

            if (student.Age is <= 0 or > 120)
                throw new ArgumentException("Age must be between 1 and 120.");

            if (student.EducationPlaceId <= 0)
                throw new ArgumentException("EducationPlaceId is invalid.");

            if (student.StatusId <= 0)
                throw new ArgumentException("StatusId is invalid.");
        }
    }
}
