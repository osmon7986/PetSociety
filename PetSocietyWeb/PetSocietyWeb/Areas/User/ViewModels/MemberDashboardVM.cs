using System.Collections.Generic;

namespace PetSocietyWeb.Areas.User.ViewModels
{
    public class MemberDashboardVM
    {
        // 數字卡片區
        public int TotalMembers { get; set; }          // 總會員數
        public int NewMembersToday { get; set; }       // 今日新增
        public int ActiveUsers24h { get; set; }        // 24小時活躍
        public int InactiveUsers { get; set; }         // 沉睡會員 (例如 >30天沒登入)

        // 排行榜區 (重複利用現有的 MemberVM)
        public List<MemberVM> TopActiveMembers { get; set; }

        // 圖表區 (給 Chart.js 用)
        public string[] ChartLabels { get; set; }      //日期 (X軸)
        public int[] ChartData { get; set; }           //註冊人數 (Y軸)
    }
}