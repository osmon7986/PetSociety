namespace PetSociety.API.DTOs.Class
{
    public class CourseDetailDTO
    {
        public int CourseDetailId { get; set; }
        public string? ImageUrl { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool Type { get; set; }
        public int TotalDuration { get; set; }
        public string? CategoryName { get; set; }
        public decimal? Price { get; set; }
        public string? InstructorName { get; set; }
        public List<CourseChapterDTO>? Chapters { get; set; }
    }
}
