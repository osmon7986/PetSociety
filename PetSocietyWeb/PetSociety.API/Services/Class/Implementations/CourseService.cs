using Humanizer;
using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Class;
using PetSociety.API.Exceptions;
using PetSociety.API.Helpers;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Class.Interfaces;
using PetSociety.API.Services.Class.Interfaces;
using System.Security.Cryptography.Xml;

namespace PetSociety.API.Services.Class.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseRecordRepository _courseRecordRepository;
        private readonly ICourseSubscribeRepository _courseSubscribeRepository;
        private readonly IChapterQuizRepository _chapterQuizRepository;
        private readonly ICourseCacheService _cache;
        private readonly PetSocietyContext _context;
       
        public CourseService(
            ICourseRepository courseRepository,
            ICourseRecordRepository courseRecordRepository,
            ICourseSubscribeRepository courseSubscribeRepository,
            IChapterQuizRepository chapterQuizRepository,
            ICourseCacheService cache,
            PetSocietyContext contex)
        {
            _courseRepository = courseRepository;
            _courseRecordRepository = courseRecordRepository;
            _courseSubscribeRepository = courseSubscribeRepository;
            _chapterQuizRepository = chapterQuizRepository;
            _cache = cache;
            _context = contex;
        }

        public async Task<CourseDetailDTO?> GetCourseDetail(int courseDetailId)
        {
            var result = await _courseRepository.GetByIdAsync(courseDetailId);
            var chapters = await _courseRepository.GetAllChaptersAsync(courseDetailId) ?? new List<CourseChapter>();

            if (result == null) // 查詢 Entity 要判斷是否 null
            {
                return null;
            }

            return new CourseDetailDTO
            {
                ImageUrl = UrlHelper.ImgFullUrl(result.ImageUrl, "courses"),
                CourseDetailId = result.CourseDetailId,
                Title = result.Course.Title,
                Description = result.Course.Description,
                Type = result.Course.Type,
                CategoryName = result.Course.Category.CategoryName,
                Price = result.Price,
                InstructorName = result.Instructor?.Name,
                Chapters = chapters.Select(c => new CourseChapterDTO
                {
                    ChapterName = c.ChapterName,
                }).ToList()
            };
        }
        public async Task<CoursePagedDTO> GetPagedCourse(CourseQueryDTO query)
        {
            // 計算 skip 數量
            int skipCount = (query.PageIndex - 1) * query.PageSize;

            // 建立查詢，尚未打DB (延遲執行 Deferred Execution)
            var queryResult = _courseRepository.Query().AsNoTracking(); // 唯獨查詢，節省記憶體，效能佳
            
            // 新增課程分類條件
            if (query.CategoryId > 0)
            {
                queryResult = queryResult
                              .Where(c => c.Course.CategoryId == query.CategoryId);
            }
            
            // 會員有輸入搜尋關鍵字 
            if (!string.IsNullOrEmpty(query.Search)) 
            {
                string searchTrim = query.Search.Trim(); // 處理空白
                queryResult = queryResult
                              .Where(c => c.Course.Title.Contains(searchTrim));
            }
            
            // 計算查詢總筆數
            int total = await queryResult.CountAsync();
            int totalPages = (int)Math.Ceiling((double)total / query.PageSize);

            // 分頁
            var result = await queryResult
                        .Skip(skipCount)
                        .Take(query.PageSize)
                        .Select(c => new
                        {
                            c.CourseDetailId,
                            c.Course.Title,
                            c.Course.Description,
                            c.Instructor.Name,
                            c.ImageUrl,
                            c.Course.Type,
                            c.DisplayBaseCount
                        })
                        .ToListAsync(); // 執行DB查詢

            return new CoursePagedDTO
            {
                Items = result.Select(r => new CourseDTO
                {
                    CourseDetailId = r.CourseDetailId,
                    Title = r.Title,
                    InstructorName = r.Name,
                    ImageUrl = UrlHelper.ImgFullUrl(r.ImageUrl, "courses"),
                    Description = r.Description,
                    Type = r.Type,
                    SubsCount = _cache.SubscriptionCounts.TryGetValue(r.CourseDetailId, out int count) ? count + r.DisplayBaseCount?? 0 : 0
                })
                .ToList(),
                Page = query.PageIndex,
                PageSize = query.PageSize,
                TotalCount = total,
                TotalPages = totalPages,
                LastUpdated = _cache.LastUpdated,
            };
        }

        public async Task<List<CourseDTO>> GetCourses()
        {
            // ToList 才打 DB
            var result = await _courseRepository.Query().ToListAsync();

            if (result.Count == 0)
            {
                return new List<CourseDTO>(); // 如果沒資料回傳空的 list
            }

            return result.Select(c => new CourseDTO
            {
                ImageUrl = UrlHelper.ImgFullUrl(c.ImageUrl, "courses"),
                Title = c.Course.Title,
                Description = c.Course.Description,
                Type = c.Course.Type,
                CourseDetailId = c.CourseDetailId,
                AreaName = c.Area?.AreaName,
                InstructorName = c.Instructor?.Name,
            })
            .ToList();
        }

        public async Task<CoursePlaybackDTO> GetPlayback(int courseDetailId, int memberId)
        {
            // 查詢會員的最後觀看章節
            var record = await _courseRepository.GetCurrentChapterAsync(memberId, courseDetailId);
            if (record == null)
            {
                // 如果會員沒有最後觀看章節，查詢此課程的第一個章節
                var firstChapterId = await _courseRepository.GetFirstChapterAsync(courseDetailId);
                if (firstChapterId <= 0)
                {
                    throw new BusinessException("課程無章節，無法建立觀看紀錄");
                }
                    
                record = new UserCourseRecord
                {
                    MemberId = memberId,
                    CourseDetailId = courseDetailId,
                    LastChapterId = firstChapterId,
                };
                await _courseRecordRepository.CreateCourseRecordAsync(record);
                await _context.SaveChangesAsync();
            }
            // 查詢課程的章節清單
            var chapters = await _courseRepository.GetAllChaptersAsync(courseDetailId);

            // 回傳 DTO 資料
            return new CoursePlaybackDTO
            {
                CourseDetailId = courseDetailId,
                CourseTitle = record.CourseDetail.Course.Title,
                CurrentChapter = new ChapterPlaybackDTO
                {
                    ChapterId = record.LastChapterId,
                    ChapterName = record.LastChapter.ChapterName,
                    ChapterSummary = record.LastChapter.Summary,
                    VideoUrl = record.LastChapter.ChapterVideos.FirstOrDefault().VideoUrl,
                },
                Chapters = chapters.Select(cc => new ChapterPlaybackDTO
                {
                    ChapterId = cc.ChapterId,
                    ChapterName = cc.ChapterName,
                    ChapterSummary = cc.Summary,
                    VideoUrl = cc.ChapterVideos.FirstOrDefault().VideoUrl,
                }).ToList()
            };
        }

        public async Task UpdateLastChapterRecord(int memberId, UpdateChapterRecordDTO dto)
        {

            // 查詢會員是否已訂閱課程
            var hasSubcription = await _courseSubscribeRepository.IsExistAsync(memberId, dto.CourseDetailId);
            if (!hasSubcription)
                throw new BusinessException("會員未訂閱該課程或訂閱已失效，無法更新觀看進度。");

            // 查詢會員觀看紀錄
            var result = await _courseRecordRepository.GetByIdAsync(memberId, dto.CourseDetailId);
            if (result == null)
                throw new BusinessException("無觀看紀錄，無法更新");

            // 更新會員觀看紀錄實體
            result.LastChapterId = dto.ChapterId;
            result.UpdatedTime = DateTime.Now;

            
            await _courseRecordRepository.UpdateAsync(result);
        }

        public async Task<List<CategoryDTO>> GetCategory()
        {
            var categories = await _courseRepository.GetCategory() ?? new List<CourseCategory>();

            // 建立"全部"選項
            var result = new List<CategoryDTO>
            {
                new CategoryDTO(0, "全部")
            };

            result.AddRange(categories
                            .Select(c => new CategoryDTO(
                                c.CategoryId, 
                                c.CategoryName)
                            ));

            return result;
        }

        public async Task<List<CourseDTO>> GetHotCourses()
        {
            // 依照總訂閱人數高到底排序，取前6個課程ID
            var hotCourseIds = _cache.HotCourseCounts
                .OrderByDescending(x => x.Value)           
                .Take(6)
                .Select(x => x.Key)
                .ToList();

            var query = _courseRepository.Query().AsNoTracking();

            // 先執行DB，只拿需要的欄位
            var list = await query
                .Where(c => hotCourseIds.Contains(c.CourseDetailId))
                .Select(c => new
                {
                    c.ImageUrl,
                    c.Course.Title,
                    c.CourseDetailId,
                    c.Instructor.Name,
                    c.DisplayBaseCount
                })
                .ToListAsync();
            
            // 再封裝進DTO
            var result = hotCourseIds // 用已排序好的 hotCourseIds 來排序
                .Select(id => list.FirstOrDefault(x => x.CourseDetailId == id))
                .Where(c => c != null) // 確保前一行不是 null
                .Select(c => new CourseDTO
                {
                    ImageUrl = UrlHelper.ImgFullUrl(c.ImageUrl,"courses"),
                    Title = c.Title,
                    CourseDetailId = c.CourseDetailId,
                    InstructorName = c.Name,
                    SubsCount = _cache.HotCourseCounts.GetValueOrDefault(c.CourseDetailId, 0) + (c.DisplayBaseCount ?? 0),
                }).ToList();
            return result;
        }

        public async Task UpdateChapterComplete(int memberId, int courseDetailId)
        {
            // 查詢會員觀看紀錄
            var result = await _courseRecordRepository.GetByIdAsync(memberId, courseDetailId);
            if (result == null)
                throw new BusinessException("無觀看紀錄，無法更新");

            result.CompletedAt = DateTime.Now;

            await _courseRecordRepository.UpdateAsync(result);
        }
    }
}
