using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Shared;
using PetSociety.API.Models;
using PetSociety.API.Services.Shared.Interfaces;
using System.Data;
using System.Security.Claims;

namespace PetSociety.API.Controllers.Class
{
    [Route("api/test_backup")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly PetSocietyContext _context;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public TestController(PetSocietyContext context, IJwtService jwtService, IConfiguration configuration)
        {
            _context = context;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        //[HttpPost("login")]
        
        //public async Task<ActionResult<TokenDTO>> Login([FromBody] LoginDTO loginDto)
        //{
        //    // 執行資料庫查詢和驗證
        //    var user = await _context.Members
        //        .FirstOrDefaultAsync(m => m.Email == loginDto.Username);
        //    if (user == null || user.Password != loginDto.Password)
        //    {
        //        return Unauthorized(new { Message = "登入失敗" }); // 401 Unauthorized
        //    }

        //    // Claim 資料
        //    var userClaims = new UserClaimsDTO
        //    {
        //        Id = user.MemberId,
        //        Username = user.Email,
        //        // Role = user.Role
        //    };

        //    // 呼叫 jwtService 生成 Token
        //    var tokenString = _jwtService.GenerateToken(userClaims);

        //    // 讀取過期時間
        //    var expiryMinutesStr = _configuration["JwtSettings:ExpireMinutes"];
        //    int.TryParse(expiryMinutesStr, out int expMinutes);

        //    // 資料封裝進 TokenDTO
        //    TokenDTO response = new TokenDTO
        //    {
        //        Token = tokenString,
        //        ExpireIn = expMinutes * 60,   // 傳入秒數
        //        TokenType = "Bearer"
        //    };

        //    return Ok(response);
        //}

        //// 測試用端點
        //[HttpGet("info")]
        //[Authorize] // 需要 JWT 才能存取，如果 JWT 無效，框架會立即返回錯誤，不會執行內部程式碼
        //public IActionResult GetMemberInfo()
        //{
        //    // 只要控制器有 [Authorize]，可以從 JWT 內部提取使用者資訊
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // ID
        //    var username = User.FindFirst(ClaimTypes.Name)?.Value; // Email
        //    //var role = User.FindFirst(ClaimTypes.Role)?.Value;

        //    return Ok(new
        //    {
        //        Message = $"歡迎！您是 role 身份的 {username}",
        //        UserId = userId
        //    });
        //}
    }
}
