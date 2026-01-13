namespace PetSociety.API.DTOs.Class
{
    
    public record QuizSubmissionDTO(int QuizId, int CourseDetailId, List<QuizAnswerDTO> Answers);
    public record QuizAnswerDTO(int QuestionId, int SelectedOptionId);
    public record QuizResultDTO(int Score, int CorrectCount, int TotalQuestions, List<QuizFeedbackDTO> Feedbacks);
    public record QuizFeedbackDTO(int QuestionId, int CorrectOptionId, bool IsCorrect, int SelectedOptionId);
    
}
