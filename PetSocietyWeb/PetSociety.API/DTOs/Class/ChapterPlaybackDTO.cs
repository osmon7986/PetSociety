namespace PetSociety.API.DTOs.Class
{
    public class ChapterPlaybackDTO
    {
        public int ChapterId { get; set; }
        public string ChapterName { get; set; }
        public string ChapterSummary { get; set; }
        public string? VideoUrl { get; set; }
    }
}
