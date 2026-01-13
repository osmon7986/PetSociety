using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSocietyWeb.Areas.User.ViewModels;
using PetSocietyWeb.Helpers;
using PetSocietyWeb.Models.Generated;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PetSocietyWeb.Areas.User.Controllers
{
    [Area("User")]
    public class MembersController : Controller
    {
        private readonly PetSocietyContext _context;

        public MembersController(PetSocietyContext context)
        {
            _context = context;
        }

        // GET: User/Members/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;
            var yesterday = DateTime.Now.AddDays(-1);
            var lastMonth = DateTime.Now.AddDays(-30);

            // 1. 準備基礎數據 (並行查詢優化效能，或分開寫也行)
            var totalMembers = await _context.Members.CountAsync();

            // 注意：資料庫若存的是 DateTime，需注意時區或時間部分，這裡簡化比對
            var newMembersToday = await _context.Members
                .Where(m => m.RegistrationDate >= today)
                .CountAsync();

            var activeUsers24h = await _context.Members
                .Where(m => m.LastloginDate >= yesterday)
                .CountAsync();

            // 定義沈睡會員：啟用中 但 最後登入時間早於30天前 (或是從未登入過且註冊超過30天)
            var inactiveUsers = await _context.Members
                .Where(m => m.IsActive && (m.LastloginDate < lastMonth || (m.LastloginDate == null && m.RegistrationDate < lastMonth)))
                .CountAsync();

            // 2. 取得登入活躍榜 (Top 5)
            var topMembers = await _context.Members
                .OrderByDescending(m => m.LoginCount)
                .Take(5)
                .Select(m => new MemberVM
                {
                    MemberId = m.MemberId,
                    Name = m.Name,
                    Email = m.Email,
                    LoginCount = m.LoginCount, // 需確認 MemberVM 有加這個欄位，若無請加上
                    LastloginDate = m.LastloginDate,
                    ProfilePic = m.ProfilePic
                })
                .ToListAsync();

            // 3. 取得近 7 天註冊趨勢圖表數據
            // GroupBy 在 SQL 轉換上有時較複雜，這裡先抓回最近7天資料再於記憶體分組
            var sevenDaysAgo = today.AddDays(-6);
            var recentMembers = await _context.Members
                .Where(m => m.RegistrationDate >= sevenDaysAgo)
                .Select(m => m.RegistrationDate)
                .ToListAsync();

            // 整理成圖表需要的陣列
            var chartLabels = new List<string>();
            var chartValues = new List<int>();

            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                chartLabels.Add(date.ToString("MM/dd")); // X軸：日期
                chartValues.Add(recentMembers.Count(d => d.Date == date)); // Y軸：該日數量
            }

            // 4. 組裝 ViewModel
            var vm = new MemberDashboardVM
            {
                TotalMembers = totalMembers,
                NewMembersToday = newMembersToday,
                ActiveUsers24h = activeUsers24h,
                InactiveUsers = inactiveUsers,
                TopActiveMembers = topMembers,
                ChartLabels = chartLabels.ToArray(),
                ChartData = chartValues.ToArray()
            };

            return View(vm);
        }

        // GET: User/Members
        public async Task<IActionResult> Index(string keyword, int? pageNumber, string sortOrder)
                {
                   
                    ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
                    ViewData["DateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "date_desc" : (sortOrder == "Date" ? "date_desc" : "Date"); ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";
                    ViewData["CurrentSort"] = sortOrder;
                    ViewData["Keyword"] = keyword; 

                    var query = _context.Members.AsQueryable();

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query = query.Where(m => m.Name.Contains(keyword) ||
                                                 m.Email.Contains(keyword) ||
                                                 m.Phone.Contains(keyword));
                    }

                    switch (sortOrder)
                    {
                        case "Name":
                            query = query.OrderBy(s => s.Name);
                            break;
                        case "name_desc":
                            query = query.OrderByDescending(s => s.Name);
                            break;
                        case "Email":
                            query = query.OrderBy(s => s.Email);
                            break;
                        case "email_desc":
                            query = query.OrderByDescending(s => s.Email);
                            break;
                        case "Date":
                            query = query.OrderBy(s => s.RegistrationDate);
                            break;
                        case "date_desc":
                            query = query.OrderByDescending(s => s.RegistrationDate);
                            break;
                        default:
                            query = query.OrderBy(s => s.RegistrationDate);
                            break;
                    }

                    var vmQuery = query.Select(m => new MemberVM
                    {
                        MemberId = m.MemberId,
                        Email = m.Email,
                        Name = m.Name,
                        Phone = m.Phone,
                        RegistrationDate = m.RegistrationDate,
                        LastloginDate = m.LastloginDate,
                        ProfilePic = m.ProfilePic,
                        IsActive = m.IsActive,
                        Role = m.Role
                    });

                    int pageSize = 10;
                    int currentPage = pageNumber ?? 1;
                    var paginatedData = await PaginatedList<MemberVM>.CreateAsync(vmQuery, currentPage, pageSize);

                    return View(paginatedData);
                }

        // GET: User/Members/Create
        public IActionResult Create()
        {
            return View(new MemberVM());
        }

        // POST: User/Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberVM vm, IFormFile newPhoto)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var model = new Member
            {
                Name = vm.Name,
                Email = vm.Email,
                Phone = vm.Phone,
                // 這裡必須加密！否則前台無法登入
                Password = HashPassword(!string.IsNullOrEmpty(vm.Password) ? vm.Password : "123456"),
                RegistrationDate = DateTime.Now,
                LastloginDate = null,
                IsActive = true,
                LoginCount = 0,
                LoginFailCount = 0,
                Role = 0,
                LoginMethod = 0,
                VerificationCode = null
            };

            if (newPhoto != null && newPhoto.Length > 0)
            {
                using var ms = new MemoryStream();
                await newPhoto.CopyToAsync(ms);
                model.ProfilePic = ms.ToArray();
            }

            _context.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

            TempData["SuccessMessage"] = "新會員建立成功！";
            return RedirectToAction(nameof(Index));
        }

        // GET: User/Members/Edit/5
        public IActionResult Edit(int id)
        {
            var m = _context.Members.Find(id);
            if (m == null) return NotFound();

            var vm = new MemberVM
            {
                MemberId = m.MemberId,
                Email = m.Email,
                Name = m.Name,
                Phone = m.Phone,
                RegistrationDate = m.RegistrationDate,
                LastloginDate = m.LastloginDate,
                ProfilePic = m.ProfilePic,
                IsActive = m.IsActive,
                Role = m.Role
            };

            return View(vm);
        }

        // POST: User/Members/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MemberVM vm, IFormFile newPhoto)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null) return NotFound();

            member.Email = vm.Email;
            member.Name = vm.Name;
            member.Phone = vm.Phone;
            member.Role = vm.Role;
            member.IsActive = vm.IsActive;

            if (!member.IsActive) member.SsoToken = null;

            bool hasPhotoUpdate = false;
            if (newPhoto != null && newPhoto.Length > 0)
            {
                using var ms = new MemoryStream();
                await newPhoto.CopyToAsync(ms);
                member.ProfilePic = ms.ToArray();
                hasPhotoUpdate = true;
            }

            await _context.SaveChangesAsync();

            // 設定成功訊息 (會存活到下一次 Request)
            TempData["SuccessMessage"] = hasPhotoUpdate ? "頭像與會員資料已更新成功！" : "會員資料已更新成功！";

            return RedirectToAction(nameof(Index));
        }

        // GET: User/Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var vm = await _context.Members
                .Select(m => new MemberVM
                {
                    MemberId = m.MemberId,
                    Email = m.Email,
                    Name = m.Name,
                    Phone = m.Phone,
                    RegistrationDate = m.RegistrationDate,
                    LastloginDate = m.LastloginDate,
                    ProfilePic = m.ProfilePic,
                    IsActive = m.IsActive,
                    Role = m.Role
                })
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (vm == null) return NotFound();

            return View(vm);
        }

        // GET: User/Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vm = await _context.Members
                .Select(m => new MemberVM
                {
                    MemberId = m.MemberId,
                    Email = m.Email,
                    Name = m.Name,
                    Phone = m.Phone,
                    RegistrationDate = m.RegistrationDate,
                    LastloginDate = m.LastloginDate,
                    ProfilePic = m.ProfilePic,
                    IsActive = m.IsActive,
                    Role = m.Role
                })
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (vm == null) return NotFound();

            return View(vm);
        }

        // POST: User/Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // API: ToggleActive
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null) return Json(new { success = false, message = "找不到會員" });

            member.IsActive = !member.IsActive;
            if (!member.IsActive) member.SsoToken = null;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = member.IsActive ? "已啟用" : "已停權" });
        }

        // API: ResetMemberPassword
        [HttpPost]
        public async Task<IActionResult> ResetMemberPassword(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null) return Json(new { success = false, message = "找不到會員" });

            member.Password = HashPassword("123456");

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "密碼已重設為 123456" });
        }

        // 圖片輸出
        public async Task<IActionResult> GetProfilePic(int id)
        {
            var members = await _context.Members.FindAsync(id);
            if (members?.ProfilePic == null) return NotFound();
            return File(members.ProfilePic, "image/jpeg");
        }

        // 密碼加密 Helper
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}