using PetSociety.API.Models;

namespace PetSociety.API.Repositories.Class.Interfaces
{
    public interface ICourseRecordRepository
    {
        /// <summary>
        /// Creates a new course record for a user.
        /// </summary>
        /// <param name="model">The <see cref="UserCourseRecord"/> containing the details of the course record to create. Cannot be
        /// <c>null</c>.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CreateCourseRecordAsync(UserCourseRecord model);
        Task<UserCourseRecord?> GetByIdAsync(int memberId, int CourseDetailId);
        /// <summary>
        /// Update UserCourseRecord
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdateAsync(UserCourseRecord model);
        
    }
}
