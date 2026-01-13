using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetSociety.API.DTOs.Shared;
using PetSociety.API.DTOs.User;
using PetSociety.API.Repositories.User.Interfaces;
using PetSociety.API.Services.Shared.Interfaces;
using PetSociety.API.Services.User.Interfaces;
using System.Security.Claims;

namespace PetSociety.API.Controllers.User
{
    [Route("api/auth")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService; 
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly IMemberRepository _repo; 

        public LoginController(
            IAuthService authService,
            IJwtService jwtService,
            IConfiguration configuration,
            IMemberRepository repo)
        {
            _authService = authService;
            _jwtService = jwtService;
            _configuration = configuration;
            _repo = repo;
        }

        // 1. 註冊 
        [HttpPost("register")]
        public async Task<ActionResult<TokenDTO>> Register(RegisterDto request)
        {
            try
            {
                var newMember = await _authService.RegisterAsync(request);

                var userClaims = new UserClaimsDTO
                {
                    Id = newMember.MemberId,
                    Username = newMember.Name,
                    Role = "User"
                };

                var tokenString = _jwtService.GenerateToken(userClaims);

                return Ok(new TokenDTO
                {
                    Token = tokenString,
                    ExpireIn = 60 * 60, // 預設 1 小時
                    TokenType = "Bearer"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. 驗證信箱 (Moved here)
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyAccount([FromBody] VerifyAccountDto request)
        {
            try
            {
                await _authService.VerifyAccountAsync(request.Email, request.Code);
                return Ok(new { message = "帳號啟用成功，現在可以登入了！" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 3. 一般登入
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);

                if (result == null)
                {
                    return Unauthorized(new { message = "帳號或密碼錯誤" });
                }

                // 產生 Token
                var tokenResult = GenerateJwtToken(result);
                return Ok(tokenResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 4. Google 登入
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto request)
        {
            try
            {
                var result = await _authService.GoogleLoginAsync(request.IdToken);

                // 產生 Token
                var tokenResult = GenerateJwtToken(result);
                return Ok(tokenResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 5. SSO Token (給後台用的)
        [HttpGet("sso-token")]
        [Authorize]
        public async Task<IActionResult> GetSsoToken()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null) return Unauthorized();

            int userId = int.Parse(userIdStr);
            var member = await _repo.GetByIdAsync(userId);

            if (member == null || member.Role != 1) return Forbid();

            var token = Guid.NewGuid().ToString();
            member.SsoToken = token;
            member.SsoTokenExpires = DateTime.Now.AddSeconds(30);
            await _repo.UpdateMemberAsync(member);

            return Ok(new { token });
        }

        // 6. 忘記密碼
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto request)
        {
            try
            {
                var message = await _authService.ForgotPasswordAsync(request.Email);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 7. 重設密碼
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto request)
        {
            try
            {
                await _authService.ResetPasswordAsync(request);
                return Ok(new { message = "密碼重設成功，請重新登入" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private object GenerateJwtToken(MemberDto member)
        {
            var userClaims = new UserClaimsDTO
            {
                Id = member.MemberId,
                Username = member.Name,
                Role = member.Role.ToString()
            };

            var tokenString = _jwtService.GenerateToken(userClaims);

            int expireMinutes = 60;
            if (int.TryParse(_configuration["JwtSettings:ExpireMinutes"], out int configMinutes))
            {
                expireMinutes = configMinutes;
            }

            return new
            {
                token = tokenString,
                expireIn = expireMinutes * 60,
                tokenType = "Bearer",
                memberId = member.MemberId,
                name = member.Name,
                email = member.Email,
                phone = member.Phone,
                role = member.Role,
                registrationDate = member.RegistrationDate, 
                lastLoginDate = member.LastLoginDate,       
                isActive = member.IsActive
            };
        }
    }
}