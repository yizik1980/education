namespace boarding_school_api.Models
{
    public class BoardingSchool
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PupilsCount { get; set; }
        public double AverageAge { get; set; }
    }
}
