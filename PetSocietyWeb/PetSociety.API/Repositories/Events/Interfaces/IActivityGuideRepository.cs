using PetSociety.API.DTOs.Events;
using PetSociety.API.Models;

namespace PetSociety.API.Repositories.Events.Interfaces
{
    public interface IActivityGuideRepository
    {
        Task<ActivityGuide> GetActivityGuide(int activityId);

        Task CreateActivityGuideAsync(ActivityGuide activityguide);
    }
}
