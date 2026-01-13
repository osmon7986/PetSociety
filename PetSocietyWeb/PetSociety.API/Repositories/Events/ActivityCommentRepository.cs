using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Events;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Events.Interfaces;

namespace PetSociety.API.Repositories.Events
{
    public class ActivityCommentRepository
    {
        public class CommentRepository : IActivityCommentRepository
        {
            private readonly PetSocietyContext _context;

            public CommentRepository(PetSocietyContext context)
            {
                _context = context;
            }

            public async Task AddAsync(ActivityComment comment)
            {
                await _context.ActivityComments.AddAsync(comment);
                await _context.SaveChangesAsync();//補UoW
            }

            public async Task<List<GetActivityCommentDto>> GetActivityComment(int activityId)
            {
                return await _context.ActivityComments
                    .Where(a => a.ActivityId == activityId)
                    .OrderBy(a => a.CreateDate)
                    .Select(a => new GetActivityCommentDto 
                    {
                        ActivityId = activityId,
                        CreateDate = a.CreateDate,
                        MemberId = a.MemberId,
                        ActivityComment1 = a.ActivityComment1,
                        MemberName = a.Member.Name
                    })
                    .ToListAsync();
            }
        }
    }
}
