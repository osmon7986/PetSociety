namespace PetSociety.API.DTOs.Class
{
    public class CourseDTO
    {
        public string ImageUrl { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Type { get; set; }
        public int TotalDuration { get; set; }
        public int CourseDetailId { get; set; }
        public string? AreaName { get; set; }
        public string? InstructorName { get; set; }
        public int SubsCount { get; set; }
        public decimal? Price { get; set; }
    }
}
