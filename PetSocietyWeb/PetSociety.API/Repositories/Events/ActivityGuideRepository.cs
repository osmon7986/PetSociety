using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Events;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Events.Interfaces;

namespace PetSociety.API.Repositories.Events
{
    public class ActivityGuideRepository : IActivityGuideRepository
    {
        private readonly PetSocietyContext _context;

        public ActivityGuideRepository(PetSocietyContext context) 
        {
            _context = context;
        }

        public async Task<ActivityGuide> GetActivityGuide(int activityId) 
        {
            return await _context.ActivityGuides
                .Where(a => a.ActivityId == activityId)
                .FirstOrDefaultAsync();
                
        }

        public async Task CreateActivityGuideAsync(ActivityGuide activityguide)
        {
            await _context.ActivityGuides.AddAsync(activityguide);
        }
    }
}
