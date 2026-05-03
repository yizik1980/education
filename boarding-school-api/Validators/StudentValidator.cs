using boarding_school_api.Data;
using boarding_school_api.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace boarding_school_api.Validators
{
    public class StudentValidator : AbstractValidator<Student>
    {
        private readonly StudentContext _context;

        public StudentValidator(StudentContext context)
        {
            _context = context;

            RuleFor(s => s.FullName)
                .NotEmpty().WithMessage("שם מלא הוא שדה חובה.")
                .MinimumLength(2).WithMessage("שם מלא חייב להכיל לפחות 2 תווים.")
                .MaximumLength(100).WithMessage("שם מלא לא יכול לעלות על 100 תווים.");

            RuleFor(s => s.NationalId)
                .NotEmpty().WithMessage("תעודת זהות היא שדה חובה.")
                .Length(9).WithMessage("תעודת זהות חייבת להכיל 9 ספרות.")
                .Matches(@"^\d{9}$").WithMessage("תעודת זהות חייבת להכיל ספרות בלבד.")
                .Must(IsValidIsraeliId).WithMessage("תעודת זהות אינה תקינה.")
                .MustAsync(async (student, nationalId, ct) =>
                    !await _context.Students.AnyAsync(
                        s => s.NationalId == nationalId && s.StudentId != student.StudentId, ct))
                .WithMessage("תעודת זהות זו כבר קיימת במערכת.");

            RuleFor(s => s.Age)
                .InclusiveBetween(6, 120).WithMessage("גיל חייב להיות בין 6 ל-120.");

            RuleFor(s => s.EducationPlaceId)
                .GreaterThan(0).WithMessage("מזהה פנימייה לא תקין.")
                .MustAsync(EducationPlaceExistsAsync)
                .WithMessage("הפנימייה שצוינה אינה קיימת במערכת.");

            RuleFor(s => s.StatusId)
                .GreaterThan(0).WithMessage("מזהה סטטוס לא תקין.");
        }

        private async Task<bool> EducationPlaceExistsAsync(int placeId, CancellationToken ct) =>
            await _context.EducationPlaces.AnyAsync(e => e.EducationPlaceId == placeId, ct);

        // Israeli ID Luhn-style checksum
        private static bool IsValidIsraeliId(string id)
        {
            if (id is not { Length: 9 } || !id.All(char.IsDigit))
                return false;

            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                int product = (id[i] - '0') * (i % 2 == 0 ? 1 : 2);
                sum += product > 9 ? product - 9 : product;
            }
            return sum % 10 == 0;
        }
    }
}
