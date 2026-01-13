using PetSociety.API.DTOs.Class;
using PetSociety.API.Exceptions;
using PetSociety.API.Helpers;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Class.Interfaces;
using PetSociety.API.Services.Class.Interfaces;

namespace PetSociety.API.Services.Class.Implementations
{
    public class CourseSubscribeService : ICourseSubscribeService
    {
        private readonly ICourseSubscribeRepository _courseSubscribeRepository;
        private readonly ICourseRecordRepository _courseRecordRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IQuizService _quizService;
        private readonly ICourseCacheService _courseCacheService;
        private readonly PetSocietyContext _context;

        public CourseSubscribeService(
            ICourseSubscribeRepository courseSubscribeRepository, 
            ICourseRepository courseRepository,
            ICourseRecordRepository courseRecordRepository,
            IQuizService quizService,
            ICourseCacheService courseCacheService,
            PetSocietyContext context)
        {
            _courseSubscribeRepository = courseSubscribeRepository;
            _courseRepository = courseRepository;
            _courseRecordRepository = courseRecordRepository;
            _quizService = quizService;
            _courseCacheService = courseCacheService;
            _context = context;
        }

        public async Task CreateSubscribe(int memberId, int courseDetailId)
        {
            // 查詢紀錄是否存在
            if (await _courseSubscribeRepository.IsExistAsync(memberId, courseDetailId))
                throw new BusinessException("此課程重複訂閱");
            // 查詢此課程的第一個章節Id
            var firstChapterId = await _courseRepository.GetFirstChapterAsync(courseDetailId);
            if (firstChapterId <= 0)
                throw new BusinessException("課程無章節，無法建立觀看紀錄");

            await using var transaction = await _context.Database.BeginTransactionAsync(); // C# 8.0 using var 寫法

            try
            {
                // 建立訂閱紀錄
                var model = new CourseSubscription
                {
                    MemberId = memberId,
                    CourseDetailId = courseDetailId,
                };
                // 新增訂閱
                await _courseSubscribeRepository.CreateSubscribeAsync(model);

                // 建立觀看紀錄
                var record = new UserCourseRecord
                {
                    MemberId = memberId,
                    CourseDetailId = courseDetailId,
                    LastChapterId = firstChapterId,
                };
                // 新增觀看紀錄
                await _courseRecordRepository.CreateCourseRecordAsync(record);
                // 統一儲存變更
                await _context.SaveChangesAsync();
                // 提交交易
                await transaction.CommitAsync();

                _courseCacheService.AddSubscription(courseDetailId);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<MyCourseDTO>> GetSubsCourses(int memberId)
        {
            // 取得會員已訂閱課程資料
            var result = await _courseSubscribeRepository.GetByMemberIdAsync(memberId);

            if (result == null || result.Count == 0)
            {
                return new List<MyCourseDTO>();
            }
            // 取得所有 courseDetailId
            var courseDetailIds = result.Select(r => r.CourseDetailId).ToList();

            var progressMap = await _quizService.GetCourseProgressBatch(memberId, courseDetailIds);

            var list = result.Select(cs => new MyCourseDTO
            {
                CourseDetailId = cs.CourseDetailId,
                ImageUrl = UrlHelper.ImgFullUrl(cs.CourseDetail.ImageUrl,"courses"),
                Title = cs.CourseDetail.Course.Title,
                // 從Dictionary取值
                ProgressPercent = progressMap.TryGetValue(cs.CourseDetailId, out var p) ? p : 0,
            }).ToList();

            return list;
                
        }

    }
}
