using Microsoft.EntityFrameworkCore;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Class.Interfaces;

namespace PetSociety.API.Repositories.Class.Implemetations
{
    public class ChapterQuizRepository : IChapterQuizRepository
    {
        private readonly PetSocietyContext _context;

        public ChapterQuizRepository(PetSocietyContext context)
        {
            _context = context;
        }
        public async Task<ChapterQuiz?> GetQuizByChapterId(int chapterId)
        {
            return await _context.ChapterQuizzes
                .AsNoTracking()
                .Include(c => c.QuizQuestions)
                    .ThenInclude(q => q.QuizOptions)
                .Where(c => c.ChapterId == chapterId)
                .FirstOrDefaultAsync();
        }
        public async Task<Dictionary<int, int>> GetCorrectAnswerAsync(int quizId)
        {
            return await _context.QuizOptions
                .AsNoTracking()
                .Where(q => q.Question.QuizId == quizId &&
                            q.IsCorrect)
                .ToDictionaryAsync(x => x.QuestionId, x => x.OptionId);

        }

        public async Task<ChapterQuizRecord?> GetQuizRecordAsync(int memberId, int quizId)
        {
            return await _context.ChapterQuizRecords
                .Where(r => r.MemberId == memberId && r.QuizId == quizId)
                .FirstOrDefaultAsync();
        }

        public async Task AddRecordAsync(ChapterQuizRecord record)
        {
            _context.ChapterQuizRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRecordAsync(ChapterQuizRecord record)
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetQuizRecordCountAsync(int memberId, int courseDetailId)
        {
            return await _context.ChapterQuizRecords
                        .Where(cq => cq.MemberId == memberId &&
                        cq.Quiz.Chapter.CourseDetailId == courseDetailId)
                        .CountAsync();
        }
    }
}
