using boarding_school_api.Data;
using boarding_school_api.Models;
using FluentValidation;

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
                .Matches(@"^\d{9}$").WithMessage("תעודת זהות חייבת להכיל ספרות בלבד.");

            RuleFor(s => s.Age)
                .InclusiveBetween(6, 120).WithMessage("גיל חייב להיות בין 6 ל-120.");

            //RuleFor(s => s.EducationPlaceId)
            //    .GreaterThan(0).WithMessage("מזהה פנימייה לא תקין.")
            //    .MustAsync(EducationPlaceExistsAsync)
            //    .WithMessage("הפנימייה שצוינה אינה קיימת במערכת.");

        }


        //private async Task<bool> EducationPlaceExistsAsync(int placeId, CancellationToken ct) =>
        //        await _context.EducationPlaces.AnyAsync(e => e.EducationPlaceId == placeId, ct);

    }
}
