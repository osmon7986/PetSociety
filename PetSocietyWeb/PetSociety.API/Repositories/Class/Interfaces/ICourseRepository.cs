using PetSociety.API.DTOs.Class;
using PetSociety.API.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PetSociety.API.Repositories.Class.Interfaces
{
    public interface ICourseRepository
    {
        /// <summary>
        /// Get Course Detail by Id
        /// </summary>
        /// <returns></returns>
        Task<CourseDetail?> GetByIdAsync(int courseDetailId);
        /// <summary>
        /// Get all Courses
        /// </summary>
        /// <returns></returns>
        IQueryable<CourseDetail> Query();
        /// <summary>
        /// Get Member's last watched chapter
        /// </summary>
        /// <returns></returns>
        Task<UserCourseRecord?> GetCurrentChapterAsync(int memberId, int courseDetailId);
        /// <summary>
        /// Get first chapter of a course
        /// </summary>
        /// <param name="courseDetailId"></param>
        /// <returns></returns>
        Task<int> GetFirstChapterAsync(int courseDetailId);
        /// <summary>
        /// Get all chapters of a course
        /// </summary>
        /// <param name="courseDetailId"></param>
        /// <returns></returns>
        Task<List<CourseChapter>> GetAllChaptersAsync(int courseDetailId);
        Task<int> GetChapterCount(int courseDetailId);
        Task<List<ChapterCountDTO>> GetChapterCountRaw(List<int> courseDetailIds);
        Task<List<CourseCategory>> GetCategory();
        /// <summary>
        /// Retrieve course detail for checkout process
        /// </summary>
        Task<CourseDetail?> GetForCheckOutAsync(int courseDetailId);
        Task AddCourseOrder(CourseOrder model);
        Task<CourseOrder?> GetOrderbyTradeNoAsync(string merchantTradeNo);
        Task<bool> HasPurchased(int memberId, int courseDetailId);
    }
}
