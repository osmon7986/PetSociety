using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSociety.API.DTOs.Events;
using PetSociety.API.Extensions;
using PetSociety.API.Services.Events.Interfaces;

namespace PetSociety.API.Controllers.Events
{
    [Route("[controller]")]
    [ApiController]
    public class ActivityCommentController : ControllerBase
    {
        private readonly IActivityCommentService _activitycommentService;

        public ActivityCommentController(IActivityCommentService activitycommentService)
        {
            _activitycommentService = activitycommentService;
        }

        //ActivityComment
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] ActivityCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var memberId = User.GetMemberId();
                dto.MemberId = memberId;
                var result = await _activitycommentService.AddCommentAsync(dto);

                // 回傳 201 Created 以及建立的資源
                return CreatedAtAction(nameof(CreateComment), new { id = result.CommentId }, result);
            }
            catch (InvalidOperationException ex)
            {
                // 捕捉審核失敗的異常，回傳 400 Bad Request
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // 處理其他未預期的錯誤
                return StatusCode(500, new { message = "伺服器發生錯誤", details = ex.Message });
            }
        }
        //ActivityComment/comment/Id
        [HttpGet("comment/{activityId}")]
        public async Task<ActionResult<List<ActivityCommentDto>>> GetComments(int activityId)
        {
            var result = await _activitycommentService.GetActivityComments(activityId);
            return Ok(result);
        }
    }
}
