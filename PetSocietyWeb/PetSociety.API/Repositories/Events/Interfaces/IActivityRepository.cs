using PetSociety.API.DTOs.Events;
using PetSociety.API.Models;

namespace PetSociety.API.Repositories.Events.Interfaces
{
    public interface IActivityRepository
    {
        //get
        Task<List<Activity>> GetAllActivities();
        Task<Activity> GetActivityInfo(int activityId);
        Task<Activity> GetActivityMap(int activityId);


        Task<List<ActivityCalenderDto>> GetActivitiesCalenders(int? memberId);

        //post
        Task CreateActivityAsync(Activity activity);
    }
}
