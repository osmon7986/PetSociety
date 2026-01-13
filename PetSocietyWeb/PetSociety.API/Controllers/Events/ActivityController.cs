using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Events;
using PetSociety.API.Extensions;
using PetSociety.API.Services.Events.Interfaces;
using System.Diagnostics;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetSociety.API.Controllers.Events
{
    [Route("[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly IActivityCalenderService _activityCalenderService;

        public ActivityController(IActivityService activityService, IActivityCalenderService activityCalenderService)
        {
            _activityService = activityService;
            _activityCalenderService = activityCalenderService;
        }



        // GET: Activity/intro
        [HttpGet("intro")]
        public async Task<ActionResult<ActivityDto>> GetActivities([FromQuery] string? category)
        {
            var result = await _activityService.GetAllActivities(category ?? string.Empty);

            // 回傳結果
            return Ok(result);
        }

        // GET: Activity/info/id
        [HttpGet("info/{activityId}")]
        public async Task<ActionResult<ActivityInfoDto>> GetActivityInfo(int activityId)
        {
            var result = await _activityService.GetActivityInfo(activityId);
            return Ok(result);
        }

        // GET: Activity/calenders
        [HttpGet("calenders")]
            
        public async Task<ActionResult<ActivityCalenderDto>> GetActivityCalenders()
        {
            int? memberId = null;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var claimId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(claimId, out int parsedId))
                {
                    memberId = parsedId;
                }
            }

            var result = await _activityCalenderService.GetActivityCalenders(memberId);
            return Ok(result);
        }

        [HttpGet("map/{activityId}")]
        public async Task<ActionResult<ActivityMapDto>> GetActivityMap(int activityId)
        {
            var result = await _activityService.GetActivityMap(activityId);
            return Ok(result);
        }

        //POST: Activity/NewActivity
        [HttpPost("NewActivity")]
        [Authorize]
        //用IActionResult較彈性，並將參數定義為Request去包含兩個dto
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityRequest request)
        {
            var memberId = User.GetMemberId();
            await _activityService.CreateActivities(
                memberId,
                request.ApplyData,
                request.GuideData
                );
            //回傳寫成json
            return Ok(new
            {
                message = "活動建立成功",
                title = request.ApplyData.Title
            });
        }

    }
}
