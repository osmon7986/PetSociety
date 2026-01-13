using Microsoft.EntityFrameworkCore;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Class.Interfaces;

namespace PetSociety.API.Repositories.Class.Implemetations
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly PetSocietyContext _context;
        public CertificateRepository(PetSocietyContext context)
        {
            _context = context;
        }

        public async Task CreateCertificate(UserCertificate model)
        {
            await _context.UserCertificates.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<UserCertificate?> GetByIds(int memberId, int courseDetailId)
        {
            return await _context.UserCertificates
                        .Include(c => c.Member)
                            .ThenInclude(m => m.UserCourseRecords)
                        .Include(c => c.CourseDetail.Course)
                        .Where(c => c.MemberId == memberId &&
                                    c.CourseDetailId == courseDetailId)
                        .FirstOrDefaultAsync();
        }
    }
}
