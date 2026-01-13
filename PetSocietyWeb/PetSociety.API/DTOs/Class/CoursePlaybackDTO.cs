namespace PetSociety.API.DTOs.Class
{
    public class CoursePlaybackDTO
    {
        public int CourseDetailId { get; set; }
        public string CourseTitle { get; set; }
        public ChapterPlaybackDTO? CurrentChapter { get; set; }
        public List<ChapterPlaybackDTO>? Chapters { get; set; }
    }
}
