using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Class;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Class.Interfaces;

namespace PetSociety.API.Repositories.Class.Implemetations
{
    public class CourseSubscribeRepository : ICourseSubscribeRepository
    {
        private readonly PetSocietyContext _context;
        public CourseSubscribeRepository(PetSocietyContext context)
        {
            _context = context;
        }
        public async Task<List<CourseSubscription>> GetByMemberIdAsync(int memberId)
        {
            return await _context.CourseSubscriptions
                        .AsNoTracking()
                        .Include(cs => cs.CourseDetail)
                            .ThenInclude(cd => cd.Course)
                        .Where(cs => cs.MemberId == memberId)
                        .OrderBy(cs => cs.SubscribeDate)
                        .ToListAsync();
        }
        public async Task CreateSubscribeAsync(CourseSubscription model)
        {
            await _context.CourseSubscriptions.AddAsync(model);
        }

        public async Task<bool> IsExistAsync(int memberId, int courseDetailId)
        {
            return await _context.CourseSubscriptions
                        .AnyAsync(cs => cs.MemberId == memberId && 
                                        cs.CourseDetailId == courseDetailId);
        }

        public async Task<ConcurrentDictionary<int, int>> GetCourseSubsCountAsync(int? days = null) // 如果沒有傳參數就當作null
        {
            // 取出 DisplayBaseCount
            var courses = await _context.CourseDetails
                    .Select(c => new
                    {
                        c.CourseDetailId,
                        c.DisplayBaseCount,
                    })
                    .ToDictionaryAsync(c => c.CourseDetailId, c => c.DisplayBaseCount ?? 0);

            var query = _context.CourseSubscriptions.AsQueryable(); // 尚未打DB

            if (days.HasValue) // 天數條件有值
            {
                DateTime startDate = DateTime.Now.AddDays(-days.Value); // 設定起始日期 (今天減去指定天數)
                query = query.Where(q => q.SubscribeDate >= startDate);
            }

            var counts =  await query
                    .GroupBy(r => r.CourseDetailId)
                    .Select(g => new
                    {
                        CourseDetailId = g.Key,
                        Count = g.Count() 
                    })
                    .ToDictionaryAsync(x => x.CourseDetailId, x => x.Count);

            var result = new ConcurrentDictionary<int, int>();

            foreach (var courseDetailId in courses.Keys)
            {
                int baseVal = courses[courseDetailId];
                int realVal = counts.GetValueOrDefault(courseDetailId, 0);

                result[courseDetailId] = baseVal + realVal;
            }

            return result;
        }


    }
}
