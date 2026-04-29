namespace boarding_school_api.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string NationalId { get; set; } = null!;
        public byte? Age { get; set; }
        public int EducationPlaceId { get; set; }
        public int StatusId { get; set; }
    }
}
