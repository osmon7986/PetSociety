using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using PetSociety.API.DTOs.Class;
using PetSociety.API.Exceptions;
using PetSociety.API.Extensions;
using PetSociety.API.Repositories.Class.Interfaces;
using PetSociety.API.Services.Class.Interfaces;

namespace PetSociety.API.Controllers.Class
{
    [Route("[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ICourseSubscribeService _courseSubscribeService;
        private readonly IQuizService _quizService;
        private readonly ICertificateService _certificateService;
        private readonly ICourseSubscribeRepository _courseSubscribeRepository;

        public CoursesController(ICourseService courseService, 
            ICourseSubscribeService courseSubscribeService,
            IQuizService quizService,
            ICertificateService certificateService,
            ICourseSubscribeRepository courseSubscribeRepository)
        {
            _courseService = courseService;
            _courseSubscribeService = courseSubscribeService;
            _quizService = quizService;
            _certificateService = certificateService;
            _courseSubscribeRepository = courseSubscribeRepository;
        }


        // 顯示分頁課程資料
        // GET: Courses/pagedCourse
        [HttpGet("pagedCourse")]
        public async Task<ActionResult<CoursePagedDTO>> GetPagedCourse([FromQuery] CourseQueryDTO dto)
        {
            var result = await _courseService.GetPagedCourse(dto);

            if (result == null)
            {
                return NotFound(); // 404
            }

            return Ok(result);
        }

        // 顯示熱門課程
        // GET: Courses/hotCourse
        [HttpGet("hotCourse")]
        [OutputCache(Duration = 600)] // 直接由 Middleware 回傳
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetHotCourses()
        {
            var result = await _courseService.GetHotCourses();

            return Ok(result);
        }

        // 顯示課程分類
        // GET: Courses/category
        [HttpGet("category")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategory()
        {
            var result = await _courseService.GetCategory();

            return Ok(result); // 200 or []
        }

        // 顯示單一課程詳細資料
        // GET: Courses/courseDetailId
        [HttpGet("{courseDetailId}")]
        public async Task<ActionResult<CourseDetailDTO>> GetCourseDetail(int courseDetailId)
        {
            var result = await _courseService.GetCourseDetail(courseDetailId);

            if (result == null)
            {
                return NotFound(); // 404
            }

            return Ok(result);
        }

        // 顯示會員所訂閱的課程資料
        // GET: Courses/myCourse
        [HttpGet("myCourse")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MyCourseDTO>>> GetSubsCourse()
        {
            var memberId = User.GetMemberId();

            var result = await _courseSubscribeService.GetSubsCourses(memberId);

            return Ok(result); // 200 or []

        }

        // 新增會員訂閱課程資料
        // POST: Courses/subscribe
        [HttpPost("subscribe")]
        [Authorize]
        public async Task<IActionResult> SubscribeCourse([FromBody] SubscribeCourseRequestDTO dto)
        {
            try
            {
                var memberId = User.GetMemberId();

                var courseDetailId = dto.CourseDetailId;

                await _courseSubscribeService.CreateSubscribe(memberId, courseDetailId);

                // 或201
                return Ok(); // 200
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400
            }
            catch (Exception ex) // 500
            {
                return StatusCode(500, new 
                {
                    message = $"系統發生錯誤，請稍後再試 {ex.Message}"
                }); 
            }

        }
        // 顯示會員課程播放畫面
        // GET: Courses/playback/courseDetailId
        [HttpGet("playback/{courseDetailId}")]
        [Authorize]
        public async Task<ActionResult<CoursePlaybackDTO>> GetCoursePlayback(int courseDetailId)
        {
            try
            {
                var memberId = User.GetMemberId();

                var result = await _courseService.GetPlayback(courseDetailId, memberId);
                return Ok(result); // 200
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400
            }
            catch (Exception ex) // 500
            {
                return StatusCode(500, new
                {
                    message = $"系統發生錯誤，請稍後再試 {ex.Message}"
                });
            }
        }

        // 更新會員章節觀看紀錄
        // PUT: Courses/playback/chapter
        [HttpPut("playback/chapter")]
        [Authorize]
        public async Task<IActionResult> UpdateChapterRecord([FromBody] UpdateChapterRecordDTO dto)
        {
            try
            {
                var memberId = User.GetMemberId();
                await _courseService.UpdateLastChapterRecord(memberId, dto);

                return NoContent(); // 204
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400
            }
            catch (Exception ex) // 500
            {
                return StatusCode(500, new
                {
                    message = $"系統發生錯誤，請稍後再試 {ex.Message}"
                });
            }
        }

        // 顯示章節測驗
        // GET: Courses/Quiz/chapterId
        [HttpGet("Quiz/{chapterId}")]
        [Authorize]
        public async Task<IActionResult> GetChapterQuiz(int chapterId)
        {
            var result = await _quizService.GetChapterQuiz(chapterId);

            if (result == null)
            {
                return NotFound(); // 404
            }

            return Ok(result); // 200
        }

        // POST: Courses/quizResult
        [HttpPost("quizResult")]
        [Authorize]
        public async Task<ActionResult<QuizResultDTO>> SubmitQuiz([FromBody]QuizSubmissionDTO submission)
        {
            var memberId = User.GetMemberId();

            var result = await _quizService.GradeQuizAsync(submission);
            await _quizService.CreateQuizRecordAsync(memberId, submission, result);
            // 查詢是否已完成所有章節測驗
            if (await _quizService.IsCompleted(memberId, submission.CourseDetailId))
            {
                // 更新課程完成日期
                await _courseService.UpdateChapterComplete(memberId, submission.CourseDetailId);
                // 新增證書
                await _certificateService.IssueUserCertificate(memberId, submission.CourseDetailId);
            }

            return Ok(result);
        }

        // 檢查會員是否有此課程訂閱紀錄
        // GET: Courses/checkSubscription/courseDetailId
        [HttpGet("checkSubscription/{courseDetailId}")]
        public async Task<bool> CheckSubscription(int courseDetailId)
        {
            var memberId = User.GetMemberId();

            return await _courseSubscribeRepository.IsExistAsync(memberId, courseDetailId);
        }
    }
}
