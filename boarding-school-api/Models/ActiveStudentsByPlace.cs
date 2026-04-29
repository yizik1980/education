namespace boarding_school_api.Models
{
    public class ActiveStudentsByPlace
    {
        public int EducationPlaceId { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public int ActiveStudentsCount { get; set; }
        public string city { get; set; } = string.Empty;
        public decimal AverageAges { get; set; }
    }
}