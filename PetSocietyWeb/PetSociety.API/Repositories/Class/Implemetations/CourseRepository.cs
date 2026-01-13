using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Class;
using PetSociety.API.Enums.Course;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Class.Interfaces;

namespace PetSociety.API.Repositories.Class.Implemetations
{
    public class CourseRepository : ICourseRepository
    {
        private readonly PetSocietyContext _context;
        public CourseRepository(PetSocietyContext context) 
        {
            _context = context;
        }

        public IQueryable<CourseDetail> Query() // 要在 Service 組合查詢條件時，要回傳 IQueryable
        {
            return _context.CourseDetails
                         .Include(cd => cd.Course)
                            .ThenInclude(c => c.Category)
                         .Include(cd => cd.Area)
                         .Include(cd => cd.Instructor)
                         .OrderBy(cd => cd.CourseDetailId);
        }

        public async Task<CourseDetail?> GetByIdAsync(int courseDetailId)
        {
            return await _context.CourseDetails
                        .Include(cd => cd.Course)
                            .ThenInclude(c => c.Category)
                        .Include(cd => cd.Area)
                        .Include(cd => cd.Instructor)
                        .Where(c => c.CourseDetailId == courseDetailId)
                        .FirstOrDefaultAsync();
        }

        public async Task<int> GetFirstChapterAsync(int courseDetailId)
        {
            return await _context.CourseChapters
                        .Where(cc => cc.CourseDetailId == courseDetailId)
                        .OrderBy(cc => cc.SortOrder)
                        .Select(cc => cc.ChapterId)
                        .FirstOrDefaultAsync();
        }

        public async Task<UserCourseRecord?> GetCurrentChapterAsync(int memberId, int courseDetailId)
        {
            return await _context.UserCourseRecords
                        .Include(cr => cr.LastChapter)
                            .ThenInclude(c => c.ChapterVideos)
                        .Include(cr => cr.CourseDetail)
                            .ThenInclude(cd => cd.Course)
                        .Where(cr => cr.MemberId == memberId && 
                                     cr.CourseDetailId == courseDetailId)
                        .FirstOrDefaultAsync();
        }

        public async Task<List<CourseChapter>> GetAllChaptersAsync(int courseDetailId)
        {
            return await _context.CourseChapters
                        .Include(cc => cc.ChapterVideos)
                        .Where(cc => cc.CourseDetailId == courseDetailId)
                        .OrderBy(cc => cc.SortOrder)
                        .ToListAsync();
        }

        public async Task<int> GetChapterCount(int courseDetailId)
        {
            return await _context.CourseChapters
                        .CountAsync(cc => cc.CourseDetailId == courseDetailId);
        }

        public async Task<List<ChapterCountDTO>> GetChapterCountRaw(List<int> courseDetailIds)
        {
            return await _context.CourseChapters
                        .AsNoTracking()
                        .Where(c => courseDetailIds.Contains(c.CourseDetailId))
                        .GroupBy(c => c.CourseDetailId)
                        .Select(g => new ChapterCountDTO
                        {
                            CourseDetailId = g.Key,
                            ChapterCount = g.Count()
                        })
                        .ToListAsync();
        }

        public async Task<List<CourseCategory>> GetCategory()
        {
            return await _context.CourseCategories
                        .OrderBy(c => c.CategoryId)
                        .ToListAsync();
        }

        // 從資料庫讀取課程結算所需資料
        public async Task<CourseDetail?> GetForCheckOutAsync(int courseDetailId)
        {
            return await _context.CourseDetails
                .AsNoTracking()
                .Where(cd => cd.CourseDetailId == courseDetailId)
                .Select(cd => new CourseDetail
                {
                    CourseDetailId = cd.CourseDetailId,
                    Price = cd.Price,
                    Course = new Course
                    {
                        Title = cd.Course.Title
                    }
                }).FirstOrDefaultAsync();
        }

        public Task AddCourseOrder(CourseOrder model)
        {
            _context.CourseOrders.Add(model);
            return _context.SaveChangesAsync();
        }

        public async Task<CourseOrder?> GetOrderbyTradeNoAsync(string merchantTradeNo)
        {
            return await _context.CourseOrders
                .Where(co => co.MerchantTradeNo == merchantTradeNo)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasPurchased(int memberId, int courseDetailId)
        {
            return await _context.CourseOrders
                .AnyAsync(o => o.BuyerMemberId == memberId &&
                o.CourseDetailId == courseDetailId &&
                o.OrderStatus == (int)OrderStatus.Paid &&
                o.IsPaid == true);
        }
    }
}
