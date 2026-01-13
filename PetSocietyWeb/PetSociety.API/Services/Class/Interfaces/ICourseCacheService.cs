using System.Collections.Concurrent;

namespace PetSociety.API.Services.Class.Interfaces
{
    // Singleton 服務
    public interface ICourseCacheService
    {
        ConcurrentDictionary<int, int> SubscriptionCounts { get; set; }
        ConcurrentDictionary<int, int> HotCourseCounts { get; set; }
        DateTime LastUpdated { get; set; } // 觀察worker工作
        public void AddSubscription(int courseDetailId);
    }
}
