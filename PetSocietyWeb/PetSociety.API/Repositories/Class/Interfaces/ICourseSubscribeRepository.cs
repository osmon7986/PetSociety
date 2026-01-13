using System.Collections.Concurrent;
using PetSociety.API.DTOs.Class;
using PetSociety.API.Models;

namespace PetSociety.API.Repositories.Class.Interfaces
{
    public interface ICourseSubscribeRepository
    {
        /// <summary>
        /// Creates a new course subscription
        /// </summary>
        /// <param name="model">The <see cref="CourseSubscription"/> object containing the details of the subscription to create. Cannot be
        /// <c>null</c>.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CreateSubscribeAsync(CourseSubscription model);
        /// <summary>
        /// Get member's subscribed courses
        /// </summary>
        /// <returns></returns>
        Task<List<CourseSubscription>> GetByMemberIdAsync(int memberId);
        /// <summary>
        /// Determines whether a subscription exists for the specified member and course detail.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="courseDetailId"></param>
        /// <returns></returns>
        Task<bool> IsExistAsync(int memberId, int courseDetailId);
        /// <summary>
        /// Count every courses subscriptions (within specific days)
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        Task<ConcurrentDictionary<int, int>> GetCourseSubsCountAsync(int? days = null);

    }
}
