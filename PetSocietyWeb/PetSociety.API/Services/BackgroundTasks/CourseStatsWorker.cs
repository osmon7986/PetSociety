
using Microsoft.VisualBasic;
using PetSociety.API.Repositories.Class.Interfaces;
using PetSociety.API.Services.Class.Interfaces;

namespace PetSociety.API.Services.BackgroundTasks
{
    public class CourseStatsWorker : BackgroundService
    {
        private readonly ICourseCacheService _cache; // 單例服務
        private readonly IServiceProvider _services;
        private readonly TimeSpan IntervalTime = TimeSpan.FromMinutes(10); // 間隔時間10分鐘
        
        public CourseStatsWorker(ICourseCacheService cache, IServiceProvider services)
        {
            _cache = cache;
            _services = services;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) 
        {
            // CancellationToken 確保程式終止時，自動跳出迴圈，並清理資料再結束，避免留下未完成的髒資料
            while (!stoppingToken.IsCancellationRequested)
            {
                // 手動建立一個生命週期範圍(scope)，請求獨立資源空間
                using (var scope = _services.CreateScope())
                {
                    // 取得 Scoped Service
                    var repo = scope.ServiceProvider.GetRequiredService<ICourseSubscribeRepository>();

                    // 呼叫方法
                    var countResult = await repo.GetCourseSubsCountAsync();
                    var hotResult = await repo.GetCourseSubsCountAsync(30);

                    _cache.SubscriptionCounts = countResult; // 將計算結果同步到快取服務
                    _cache.HotCourseCounts = hotResult;
                    _cache.LastUpdated = DateTime.Now;
                }
                Console.WriteLine(">>> [PetSociety] 快取更新完成！準備休息 10 分鐘。");
                // 設定每 10 分鐘 Delay，10 分鐘結束後才進入下一次 while 迴圈
                // Task.Delay 非同步，不會占用 CPU 資源
                await Task.Delay(IntervalTime, stoppingToken); // 傳入 CancellationToken，告訴 Task.Delay 可以中斷工作
            }
        }
    }
}
