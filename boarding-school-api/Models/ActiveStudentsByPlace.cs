namespace boarding_school_api.Models
{
    public class ActiveStudentsByPlace
    {
        public int EducationPlaceId { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public int ActiveStudentCount { get; set; }
        public string city { get; set; } = string.Empty;
        public decimal AvrageAges { get; set; }
    }
}