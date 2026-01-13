using PetSociety.API.DTOs.Class;

namespace PetSociety.API.Services.Class.Interfaces
{
    public interface ICourseSubscribeService
    {
        /// <summary>
        /// Get member's subscribed courses
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task<List<MyCourseDTO>> GetSubsCourses(int memberId);
        /// <summary>
        /// Create a new course subscription with associated course record
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="courseDetailId"></param>
        /// <returns></returns>
        Task CreateSubscribe(int memberId, int courseDetailId);
        
    }
}
