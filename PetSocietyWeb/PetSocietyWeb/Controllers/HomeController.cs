using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSocietyWeb.Models;
using PetSocietyWeb.Models.Generated;
using System.Security.Claims;
using Activity = System.Diagnostics.Activity;

namespace PetSocietyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PetSocietyContext _context;

        private const string AngularUrl = "http://localhost:4200";
        private const string AngularLoginUrl = "http://localhost:4200/member/login";

        public HomeController(ILogger<HomeController> logger, PetSocietyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect(AngularLoginUrl);
            }
            return View();
        }

        public async Task<IActionResult> SsoLogin(string token)
        {
            // 沒 Token 直接回 Angular
            if (string.IsNullOrEmpty(token)) return Redirect(AngularLoginUrl);

            // 1. 檢查 Token 是否有效且未過期
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.SsoToken == token && m.SsoTokenExpires > DateTime.Now);

            if (member == null)
            {
                // Token 無效，導回 Angular 登入頁 (通常代表連結過期或已使用)
                return Redirect(AngularLoginUrl);
            }

            // 2. 建立使用者的身分證 (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, member.Name ?? "Admin"),
                new Claim(ClaimTypes.Email, member.Email),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.NameIdentifier, member.MemberId.ToString()),
                new Claim("MemberId", member.MemberId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
            };

            // 3. 發送 Cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // 4. 銷毀 Token
            member.SsoToken = null;
            await _context.SaveChangesAsync();

            // 5. 導向到後台首頁
            return RedirectToAction("Index", "Home" );
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // 無權限時，導回 Angular 登入頁
        public IActionResult NoPermission()
        {
            return Redirect(AngularLoginUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // 登出後，導回 Angular 登入頁
        public async Task<IActionResult> Logout()
        {
            // 清除 Cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 導向 Angular
            return Redirect(AngularLoginUrl);
        }
    }
}