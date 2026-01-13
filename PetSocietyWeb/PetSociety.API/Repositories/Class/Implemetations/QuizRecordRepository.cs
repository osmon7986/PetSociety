using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Class;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Class.Interfaces;

namespace PetSociety.API.Repositories.Class.Implemetations
{
    public class QuizRecordRepository : IQuizRecordRepository
    {
        private readonly PetSocietyContext _context;
        public QuizRecordRepository(PetSocietyContext context)
        {
            _context = context;
        }
        public Task<int> CountQuizRecord(int memberId, int courseDetailId)
        {
            // MemberId & CourseDetailId Unique 索引鍵，不用 Distinct
            return _context.ChapterQuizRecords
                    .CountAsync(r => r.MemberId == memberId &&
                                     r.Quiz.Chapter.CourseDetailId == courseDetailId);
        }

        public async Task<List<CourseProgressDTO>> GetCompletedCourseRaw(int memberId, List<int> courseDetailId)
        {
            return await _context.ChapterQuizRecords
                        .AsNoTracking()
                        .Where(r => r.MemberId == memberId &&
                                    courseDetailId.Contains(r.Quiz.Chapter.CourseDetailId))
                        .GroupBy(r => r.Quiz.Chapter.CourseDetailId) // Groupby CourseDetailId
                        .Select(g => new CourseProgressDTO
                        {
                            CourseDetailId = g.Key,
                            CompletedCount = g.Count()
                        })
                        .ToListAsync();
        }

        
    }
}
