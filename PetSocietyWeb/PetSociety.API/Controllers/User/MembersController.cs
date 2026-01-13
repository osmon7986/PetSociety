using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetSociety.API.DTOs.User;
using PetSociety.API.Services.User.Interfaces;
using System.Security.Claims;

namespace PetSociety.API.Controllers.User
{
    [Route("api/members")]
    [ApiController]
    [Authorize] 
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService; 

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        // 1. 取得個人資料 (GET api/members/profile)
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var memberIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(memberIdStr) || !int.TryParse(memberIdStr, out int memberId))
            {
                return Unauthorized("無法識別使用者");
            }

            var memberDto = await _memberService.GetMemberAsync(memberId);

            if (memberDto == null)
            {
                return NotFound("找不到該會員");
            }

            return Ok(memberDto);
        }

        // 2. 修改密碼 (PUT api/members/password)
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto request)
        {
            var memberIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(memberIdStr) || !int.TryParse(memberIdStr, out int memberId))
            {
                return Unauthorized("無法識別使用者");
            }

            try
            {
                await _memberService.ChangePasswordAsync(memberId, request);
                return Ok(new { message = "密碼修改成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 3. 修改個人資料 (PUT api/members/{id})
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileDto request)
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claimId))
            {
                return Unauthorized("Token 無效，找不到使用者 ID");
            }

            int currentUserId = int.Parse(claimId);

            // 防止 A 用戶去改 B 用戶的資料
            if (id != currentUserId)
            {
                return StatusCode(403, "你無權修改其他使用者的資料");
            }

            try
            {
                await _memberService.UpdateMemberAsync(id, request);
                return Ok(new { message = "資料更新成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}