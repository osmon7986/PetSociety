using PetSociety.API.DTOs.Events;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Events;
using PetSociety.API.Repositories.Events.Interfaces;
using PetSociety.API.Services.Events.Interfaces;

namespace PetSociety.API.Services.Events
{
    public class ActivityGuideService : IActivityGuideService
    {
        private readonly IActivityGuideRepository _IActivityGuideRepository;
        private readonly IUnitOfWork _IunitOfWork;
        public ActivityGuideService(IActivityGuideRepository activityGuideRepo,IUnitOfWork unitOfWork) 
        {
            _IActivityGuideRepository = activityGuideRepo;
            _IunitOfWork = unitOfWork;
        }

        public async Task<ActivityGuideDto> GetActivityGuide(int activityId)
        {
            //從repository層取Enitity
            var result = await _IActivityGuideRepository.GetActivityGuide(activityId);
            if (result == null)
            {
                return null;
            }
            { 
                //有資料才進行Entity轉換dto
                return new ActivityGuideDto
                {
                    ActivityId = result.ActivityId,
                    GuideId = result.GuideId,
                    ActivityEditorHtml = result.ActivityEditorHtml,
                };
            }
        }

        public async Task<ActivityGuideDto> PostActivityGuide(ActivityGuideDto dto)
        {
            //new entity
            var newActivityGuide = new ActivityGuide
            {
                ActivityId = dto.ActivityId,
                ActivityEditorHtml = dto.ActivityEditorHtml,
                CreatedAt = DateTime.Now,
            };

            await _IActivityGuideRepository.CreateActivityGuideAsync(newActivityGuide);
            await _IunitOfWork.CompleteAsync();
            return dto;
        }
    }
}
