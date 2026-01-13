using PetSociety.API.DTOs.Class;
using PetSociety.API.Models;

namespace PetSociety.API.Repositories.Class.Interfaces
{
    public interface IQuizRecordRepository
    {
        /// <summary>
        /// Get a member's quiz records
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task<int> CountQuizRecord(int memberId, int courseDetailId);
        Task<List<CourseProgressDTO>> GetCompletedCourseRaw(int memberId, List<int> courseDetailIds);
        
    }
}
