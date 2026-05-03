using boarding_school_api.Data;
using boarding_school_api.Models;
using boarding_school_api.Validators;
using Microsoft.EntityFrameworkCore;

namespace boarding_school_api.Tests;

public class StudentValidatorTests : IDisposable
{
    private readonly StudentContext _context;
    private readonly StudentValidator _validator;

    public StudentValidatorTests()
    {
        var options = new DbContextOptionsBuilder<StudentContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new StudentContext(options);

        _context.EducationPlaces.Add(new EducationPlaceSummary
        {
            EducationPlaceId = 1,
            PlaceName = "פנימייה לבדיקות",
            ActiveStudentCount = 10,
            city = "תל אביב",
            AvrageAges = 15
        });
        _context.SaveChanges();

        _validator = new StudentValidator(_context);
    }

    public void Dispose() => _context.Dispose();

    // ── FullName ──────────────────────────────────────────────────────────

    [Fact]
    public async Task FullName_Empty_Fails()
    {
        var result = await _validator.ValidateAsync(Valid(s => s.FullName = ""));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FullName" && e.ErrorMessage.Contains("שדה חובה"));
    }

    [Fact]
    public async Task FullName_TooShort_Fails()
    {
        var result = await _validator.ValidateAsync(Valid(s => s.FullName = "א"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FullName");
    }

    [Fact]
    public async Task FullName_TooLong_Fails()
    {
        var result = await _validator.ValidateAsync(Valid(s => s.FullName = new string('א', 101)));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FullName");
    }

    // ── NationalId ────────────────────────────────────────────────────────

    [Fact]
    public async Task NationalId_WrongLength_Fails()
    {
        var result = await _validator.ValidateAsync(Valid(s => s.NationalId = "12345"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "NationalId");
    }

    [Fact]
    public async Task NationalId_NonNumeric_Fails()
    {
        var result = await _validator.ValidateAsync(Valid(s => s.NationalId = "12345678a"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "NationalId");
    }

    [Fact]
    public async Task NationalId_InvalidChecksum_Fails()
    {
        // 123456789 — 9 digits but Luhn sum = 47, not divisible by 10
        var result = await _validator.ValidateAsync(Valid(s => s.NationalId = "123456789"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "NationalId" && e.ErrorMessage.Contains("אינה תקינה"));
    }

    [Fact]
    public async Task NationalId_DuplicateInDb_Fails()
    {
        _context.Students.Add(new Student
        {
            StudentId = 10, FullName = "קיים", NationalId = "123456782",
            Age = 16, EducationPlaceId = 1, StatusId = 1
        });
        await _context.SaveChangesAsync();

        // New student (StudentId=0) with same NationalId
        var result = await _validator.ValidateAsync(Valid(s => s.StudentId = 0));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "NationalId" && e.ErrorMessage.Contains("כבר קיימת"));
    }

    [Fact]
    public async Task NationalId_SameStudentUpdate_Passes()
    {
        _context.Students.Add(new Student
        {
            StudentId = 5, FullName = "קיים", NationalId = "123456782",
            Age = 16, EducationPlaceId = 1, StatusId = 1
        });
        await _context.SaveChangesAsync();

        // Updating same student with its own NationalId — should not be flagged as duplicate
        var result = await _validator.ValidateAsync(Valid(s => s.StudentId = 5));
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "NationalId" && e.ErrorMessage.Contains("כבר קיימת"));
    }

    // ── Age ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Age_Below6_Fails()
    {
        var result = await _validator.ValidateAsync(Valid(s => s.Age = 5));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Age");
    }

    [Fact]
    public async Task Age_Above120_Fails()
    {
        var result = await _validator.ValidateAsync(Valid(s => s.Age = 121));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Age");
    }

    // ── EducationPlaceId ──────────────────────────────────────────────────

    [Fact]
    public async Task EducationPlaceId_Zero_Fails()
    {
        var result = await _validator.ValidateAsync(Valid(s => s.EducationPlaceId = 0));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EducationPlaceId" && e.ErrorMessage.Contains("לא תקין"));
    }

    [Fact]
    public async Task EducationPlaceId_NotInDb_Fails()
    {
        var result = await _validator.ValidateAsync(Valid(s => s.EducationPlaceId = 999));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EducationPlaceId" && e.ErrorMessage.Contains("אינה קיימת"));
    }

    [Fact]
    public async Task EducationPlaceId_ExistsInDb_NoError()
    {
        var result = await _validator.ValidateAsync(ValidStudent());
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "EducationPlaceId");
    }

    // ── StatusId ──────────────────────────────────────────────────────────

    [Fact]
    public async Task StatusId_Zero_Fails()
    {
        var result = await _validator.ValidateAsync(Valid(s => s.StatusId = 0));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "StatusId");
    }

    // ── Full valid student ─────────────────────────────────────────────────

    [Fact]
    public async Task ValidStudent_PassesAllRules()
    {
        var result = await _validator.ValidateAsync(ValidStudent());
        Assert.True(result.IsValid);
    }

    // ── helpers ───────────────────────────────────────────────────────────

    private static Student ValidStudent() => new()
    {
        StudentId = 0,
        FullName = "ישראל ישראלי",
        NationalId = "123456782",  // Luhn sum = 40 ✓
        Age = 16,
        EducationPlaceId = 1,
        StatusId = 1
    };

    private static Student Valid(Action<Student> mutate)
    {
        var s = ValidStudent();
        mutate(s);
        return s;
    }
}
