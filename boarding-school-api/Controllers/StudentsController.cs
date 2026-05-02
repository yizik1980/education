using boarding_school_api.Infrastructure;
using boarding_school_api.Models;
using boarding_school_api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class StudentsController(
    IStudentsRepository studentRepository,
    IValidator<Student> validator,
    ILoggingService loggingService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await studentRepository.GetAllStudentsAsync());

    [HttpGet("place/{placeId}")]
    public async Task<IActionResult> GetByPlace(int placeId) =>
        Ok(await studentRepository.GetStudentsByPlaceIdAsync(placeId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Student student)
    {
        var validation = await validator.ValidateAsync(student);
        if (!validation.IsValid)
        {
            var errors = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));
            await loggingService.LogAsync("WARN", $"[POST /api/students] Validation failed: {errors}");
            return BadRequest(new
            {
                errors = validation.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage })
            });
        }

        var created = await studentRepository.InsertNewStudent(student);
        return CreatedAtAction(nameof(GetAll), new { id = created }, created);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Student student)
    {
        if (student.StudentId <= 0)
        {
            await loggingService.LogAsync("WARN", $"[PUT /api/students] Validation failed: מזהה תלמיד לא תקין.");
            return BadRequest(new
            {
                errors = new[] { new { property = nameof(Student.StudentId), message = "מזהה תלמיד לא תקין." } }
            });
        }

        var validation = await validator.ValidateAsync(student);
        if (!validation.IsValid)
        {
            var errors = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));
            await loggingService.LogAsync("WARN", $"[PUT /api/students/{student.StudentId}] Validation failed: {errors}");
            return BadRequest(new
            {
                errors = validation.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage })
            });
        }

        var updated = await studentRepository.UpdateStudent(student);
        return Ok(updated);
    }

    [HttpDelete("{studentId}")]
    public async Task<IActionResult> Delete(int studentId)
    {
        await studentRepository.DeleteStudent(studentId);
        return NoContent();
    }
}