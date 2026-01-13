using Microsoft.EntityFrameworkCore;
using PetSocietyWeb.Models.Generated;

namespace PetSocietyWeb.Areas.Class.Repositories
{
    public class CourseRepository
    {
        private readonly PetSocietyContext _context;

        public CourseRepository (PetSocietyContext context)
        {
            _context = context;
        }

        public async Task CreateCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task<Course> GetCourseAsync(int id)
        {
            return await _context.Courses
                   .Include(c => c.CourseDetails)
                       .ThenInclude(d => d.CourseChapters)
                           .ThenInclude(ch => ch.ChapterVideos)
                   .FirstOrDefaultAsync(c => c.CourseId == id);
        }

        // 刪除所有影片
        public void RemoveVideos(IEnumerable<CourseChapter> chapters)
        {
            foreach (var chapter in chapters)
            {
                _context.ChapterVideos.RemoveRange(chapter.ChapterVideos);
            }
        }

        // 刪除章節
        public void RemoveChapters(IEnumerable<CourseChapter> chapters)
        {
            _context.CourseChapters.RemoveRange(chapters);
        }

        // 刪除 Detail
        public void RemoveDetails(IEnumerable<CourseDetail> details)
        {
            _context.CourseDetails.RemoveRange(details);
        }

        // 刪除 Course
        public void RemoveCourse(Course course)
        {
            _context.Courses.Remove(course);
        }

        // Save
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
