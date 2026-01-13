namespace PetSociety.API.DTOs.Class
{
    public class QuizQuestionDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<QuizOptionDTO> Options { get; set; } = new();
    }
}
