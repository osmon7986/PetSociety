using PetSociety.API.DTOs.Class;

namespace PetSociety.API.Services.Class.Interfaces
{
    public interface ICourseService
    {
        /// <summary>
        /// Get All Courses
        /// </summary>
        /// <returns></returns>
        Task<List<CourseDTO>> GetCourses();
        /// <summary>
        /// Get Courses by pages and keyword
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<CoursePagedDTO> GetPagedCourse(CourseQueryDTO query);
        /// <summary>
        /// Get Course Detail by courseDetailId
        /// </summary>
        /// <returns></returns>
        Task<CourseDetailDTO?> GetCourseDetail(int courseDetailId);
        /// <summary>
        /// Get playback data by ids
        /// </summary>
        /// <param name="courseDetailId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task<CoursePlaybackDTO> GetPlayback(int courseDetailId, int memberId);
        /// <summary>
        /// Update user's last watched chapter
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task UpdateLastChapterRecord(int memberId, UpdateChapterRecordDTO dto);
        Task UpdateChapterComplete(int memberId, int courseDetailId);
        Task<List<CategoryDTO>> GetCategory();
        Task<List<CourseDTO>> GetHotCourses();
    }
}
