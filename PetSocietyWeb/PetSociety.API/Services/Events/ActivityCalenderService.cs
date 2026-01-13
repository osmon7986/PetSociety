using Microsoft.AspNetCore.Mvc;
using PetSociety.API.DTOs.Events;
using PetSociety.API.Repositories.Events.Interfaces;
using PetSociety.API.Services.Events.Interfaces;

namespace PetSociety.API.Services.Events
{
    public class ActivityCalenderService :IActivityCalenderService
    {
        //查詢同一張表且類型相似故用同一組repository
        private readonly IActivityRepository _IActivityRepository;

        public ActivityCalenderService(IActivityRepository activityCalenderRepositoy)
        {
            _IActivityRepository = activityCalenderRepositoy; ;
        }

        public async Task<List<ActivityCalenderDto>> GetActivityCalenders(int? memberId)
        {
            var result = await _IActivityRepository.GetActivitiesCalenders(memberId);
            return result;
        }
    }
}
