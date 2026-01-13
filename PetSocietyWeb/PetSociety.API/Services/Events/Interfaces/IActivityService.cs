using PetSociety.API.DTOs.Events;
using PetSociety.API.Models;


namespace PetSociety.API.Services.Events.Interfaces
{
    public interface IActivityService
    {

        Task<List<ActivityDto>> GetAllActivities(string category);
        Task<ActivityInfoDto> GetActivityInfo(int activityId);
        Task<ActivityMapDto> GetActivityMap(int activityId);


        Task CreateActivities(int memberId, ActivityApplyDto dto, ActivityGuideDto guidedto);

        
    }
}
