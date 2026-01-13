using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PetSocietyWeb.Areas.User.ViewModels
{
    public class MemberVM
    {
        public int MemberId { get; set; }

        [Required(ErrorMessage = "請輸入 Email")]
        [EmailAddress(ErrorMessage = "Email 格式不正確")]
        [StringLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "請輸入名字")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "名字至少 3 個字")]
        public string Name { get; set; }

        // [修改] 移除 [BindNever]，這樣管理員在 Create 頁面輸入密碼才有效
        // 如果是 Edit 頁面不想讓人改密碼，可以在 View 裡面不顯示這個欄位，或是 Controller 做判斷
        public string? Password { get; set; }

        [Required(ErrorMessage = "請輸入聯絡電話")]
        [RegularExpression(@"^09\d{2}[- ]?\d{3}[- ]?\d{3}$", ErrorMessage = "電話格式需為 09 開頭，共 10 碼")]
        public string Phone { get; set; }

        public bool IsActive { get; set; }

        public int Role { get; set; }

        public int LoginCount { get; set; }
        public DateTime? LastloginDate { get; set; }
        public DateTime? RegistrationDate { get; set; } // 建議改成可空 (DateTime?) 避免轉換錯誤

        // ↓↓↓↓↓ 這些欄位絕對要 BindNever (純顯示用)
        [BindNever]
        public byte[]? ProfilePic { get; set; }

        [BindNever]
        public string? ProfilePicBase64 =>
            ProfilePic != null ? $"data:image/png;base64,{Convert.ToBase64String(ProfilePic)}" : null;

        [BindNever]
        public string? RegistrationDateString => RegistrationDate?.ToString("yyyy-MM-dd");

        [BindNever]
        public string? LastLoginDateString => LastloginDate?.ToString("yyyy-MM-dd HH:mm");
    }
}