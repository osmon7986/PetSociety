using Microsoft.EntityFrameworkCore;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Class.Interfaces;

namespace PetSociety.API.Repositories.Class.Implemetations
{
    public class CourseRecordRepository : ICourseRecordRepository
    {
        private readonly PetSocietyContext _context;
        public CourseRecordRepository(PetSocietyContext context)
        {
            _context = context;
        }
        public async Task CreateCourseRecordAsync(UserCourseRecord model)
        {
            await _context.UserCourseRecords.AddAsync(model);
        }

        public async Task<UserCourseRecord?> GetByIdAsync(int memberId, int courseDetailId)
        {
            return await _context.UserCourseRecords
                        .Where(cs => cs.MemberId == memberId &&
                                     cs.CourseDetailId == courseDetailId)
                        .FirstOrDefaultAsync();
        }

        

        public async Task UpdateAsync(UserCourseRecord model)
        {
            _context.UserCourseRecords.Update(model);

            await _context.SaveChangesAsync();
        }
    }
}
