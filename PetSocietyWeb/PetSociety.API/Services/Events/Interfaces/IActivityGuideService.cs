using PetSociety.API.DTOs.Events;

namespace PetSociety.API.Services.Events.Interfaces
{
    public interface IActivityGuideService
    {
        Task<ActivityGuideDto> GetActivityGuide(int activityId);
        Task<ActivityGuideDto> PostActivityGuide(ActivityGuideDto dto);
    }
}
