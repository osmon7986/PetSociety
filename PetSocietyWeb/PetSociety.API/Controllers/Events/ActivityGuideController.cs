using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSociety.API.DTOs.Events;
using PetSociety.API.Services.Events;
using PetSociety.API.Services.Events.Interfaces;

namespace PetSociety.API.Controllers.Events
{
    [Route("[controller]")]
    [ApiController]
    public class ActivityGuideController : ControllerBase
    {
        private readonly IActivityGuideService _activityGuideService;

        public ActivityGuideController(IActivityGuideService activityGuideService)
        {
            _activityGuideService = activityGuideService;
        }

        //GET: ActivityGuide/id
        [HttpGet("{activityId}")]
        public async Task<ActionResult<ActivityGuideDto>> GetActivityGuide(int activityId)
        {
            
            var result = await _activityGuideService.GetActivityGuide(activityId);

            return Ok(result);
        }
        //Post: ActivityGuide/GuideEditor
        [HttpPost("GuideEditor")]
        public async Task<ActionResult<ActivityGuideDto>> PostActivityGuide(ActivityGuideDto activityGuideDto)
        {
            var result = await _activityGuideService.PostActivityGuide(activityGuideDto);
            return Ok(result);
        }
    }
}
