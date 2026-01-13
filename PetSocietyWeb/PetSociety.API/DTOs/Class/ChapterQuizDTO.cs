namespace PetSociety.API.DTOs.Class
{
    public class ChapterQuizDTO
    {
        public int QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<QuizQuestionDTO> Questions { get; set; } = new();
    }
}
