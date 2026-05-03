using boarding_school_api.Infrastructure;
using boarding_school_api.Models;
using boarding_school_api.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace boarding_school_api.Tests;

public class StudentsControllerTests
{
    private readonly Mock<IStudentsRepository> _repoMock = new();
    private readonly Mock<IValidator<Student>> _validatorMock = new();
    private readonly Mock<ILoggingService> _loggerMock = new();
    private readonly StudentsController _controller;

    public StudentsControllerTests()
    {
        _controller = new StudentsController(_repoMock.Object, _validatorMock.Object, _loggerMock.Object);
    }

    // ── GET /api/students ──────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ReturnsOkWithStudents()
    {
        var students = new[] { MakeStudent(1), MakeStudent(2) }.AsQueryable();
        _repoMock.Setup(r => r.GetAllStudentsAsync()).ReturnsAsync(students);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    // ── GET /api/students/place/{placeId} ──────────────────────────────────

    [Fact]
    public async Task GetByPlace_ReturnsOkWithStudents()
    {
        var students = new[] { MakeStudent(1, placeId: 5) }.AsQueryable();
        _repoMock.Setup(r => r.GetStudentsByPlaceIdAsync(5)).ReturnsAsync(students);

        var result = await _controller.GetByPlace(5);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    // ── POST /api/students ─────────────────────────────────────────────────

    [Fact]
    public async Task Create_ValidationFails_Returns400WithErrors()
    {
        var student = MakeStudent(0);
        var failures = new List<ValidationFailure>
        {
            new("FullName", "שם מלא הוא שדה חובה.")
        };
        _validatorMock.Setup(v => v.ValidateAsync(student, default))
                      .ReturnsAsync(new ValidationResult(failures));

        var result = await _controller.Create(student);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, bad.StatusCode);
    }

    [Fact]
    public async Task Create_ValidationPasses_Returns201Created()
    {
        var student = MakeStudent(0);
        _validatorMock.Setup(v => v.ValidateAsync(student, default))
                      .ReturnsAsync(new ValidationResult());
        _repoMock.Setup(r => r.InsertNewStudent(student))
                 .ReturnsAsync(new List<Student> { student });

        var result = await _controller.Create(student);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
    }

    // ── PUT /api/students ──────────────────────────────────────────────────

    [Fact]
    public async Task Update_StudentIdZero_Returns400()
    {
        var student = MakeStudent(0);

        var result = await _controller.Update(student);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, bad.StatusCode);
    }

    [Fact]
    public async Task Update_ValidationFails_Returns400()
    {
        var student = MakeStudent(1);
        var failures = new List<ValidationFailure>
        {
            new("Age", "גיל חייב להיות בין 6 ל-120.")
        };
        _validatorMock.Setup(v => v.ValidateAsync(student, default))
                      .ReturnsAsync(new ValidationResult(failures));

        var result = await _controller.Update(student);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, bad.StatusCode);
    }

    [Fact]
    public async Task Update_ValidationPasses_Returns200Ok()
    {
        var student = MakeStudent(1);
        _validatorMock.Setup(v => v.ValidateAsync(student, default))
                      .ReturnsAsync(new ValidationResult());
        _repoMock.Setup(r => r.UpdateStudent(student)).ReturnsAsync(student);

        var result = await _controller.Update(student);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    // ── DELETE /api/students/{id} ──────────────────────────────────────────

    [Fact]
    public async Task Delete_Returns204NoContent()
    {
        _repoMock.Setup(r => r.DeleteStudent(1)).Returns(Task.CompletedTask);

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    // ── helpers ───────────────────────────────────────────────────────────

    private static Student MakeStudent(int id, int placeId = 1) => new()
    {
        StudentId = id,
        FullName = "ישראל ישראלי",
        NationalId = "123456782",
        Age = 16,
        EducationPlaceId = placeId,
        StatusId = 1
    };
}
