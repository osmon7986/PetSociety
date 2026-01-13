using System.Collections.Concurrent;
using PetSociety.API.Services.Class.Interfaces;

namespace PetSociety.API.Services.Class.Implementations
{
    public class CourseCacheService : ICourseCacheService

    {
        public ConcurrentDictionary<int, int> SubscriptionCounts { get; set; } = new();
        public ConcurrentDictionary<int, int> HotCourseCounts { get; set; } = new();
        public DateTime LastUpdated { get; set; }

        public void AddSubscription(int courseDetailId)
        {
            SubscriptionCounts.AddOrUpdate(courseDetailId, 1, (id, oldValue) => oldValue + 1);
            LastUpdated = DateTime.Now;
        }
    }
}
