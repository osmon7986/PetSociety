using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Events;
using PetSociety.API.Enums.Events;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Events.Interfaces;

namespace PetSociety.API.Repositories.Events
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly PetSocietyContext _context;

        public ActivityRepository(PetSocietyContext context)
        {
            _context = context;
        }

        

        public async Task<List<Activity>> GetAllActivities()
        {

            //DateTime halfyearFuture = DateTime.Now.AddDays(180);
            return await _context.Activities
                .Include(a => a.Tags)
                    .ThenInclude(t => t.TagCategory)
                //¿z¿ïActivityCheckª¬ºA(0:Pending,1:Published,2:Rejected)
                .Where(a => a.ActivityCheck == (int)ActivityStatus.Published)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<Activity> GetActivityInfo(int activityId)
        {

            //DateTime halfyearFuture = DateTime.Now.AddDays(180);
            return await _context.Activities
                .Where(a => a.ActivityId == activityId)
                .FirstOrDefaultAsync();
        }

        public async Task<Activity> GetActivityMap(int activityId)
        {

            //DateTime halfyearFuture = DateTime.Now.AddDays(180);
            return await _context.Activities
                .Where(a => a.ActivityId == activityId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ActivityCalenderDto>> GetActivitiesCalenders(int? memberId)
        {
            return await _context.Activities
                .Select(a => new ActivityCalenderDto
                {
                    ActivityId = a.ActivityId,
                    Title = a.Title,
                    StartTime = a.StartTime,
                    IsRegistered = memberId.HasValue
                        && a.Participants.Any(p => p.MemberId==memberId) // Placeholder, actual logic to determine registration status needed
                })
                .ToListAsync();
        }

        public async Task CreateActivityAsync(Activity activity)
        {
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
        }


    }
}