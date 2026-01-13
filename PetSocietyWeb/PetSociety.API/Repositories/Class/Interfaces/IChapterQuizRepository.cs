using PetSociety.API.Models;

namespace PetSociety.API.Repositories.Class.Interfaces
{
    public interface IChapterQuizRepository
    {
        Task<ChapterQuiz?> GetQuizByChapterId(int chapterId);
        Task<Dictionary<int, int>> GetCorrectAnswerAsync(int quizId);
        Task<ChapterQuizRecord?> GetQuizRecordAsync(int memberId, int quizId);
        Task<int> GetQuizRecordCountAsync(int memberId, int courseDetailId);
        Task AddRecordAsync(ChapterQuizRecord record);
        Task UpdateRecordAsync(ChapterQuizRecord record);
    }
} 
