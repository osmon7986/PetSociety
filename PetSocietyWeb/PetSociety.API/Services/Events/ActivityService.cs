using Humanizer;
using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Events;
using PetSociety.API.Helpers;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Events.Interfaces;
using PetSociety.API.Services.Events.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PetSociety.API.Services.Events
{
    
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _IActivityRepository;
        private readonly IActivityGuideRepository _IActivityGuideRepository;
        private readonly IUnitOfWork _IunitOfWork;

        public ActivityService(IActivityRepository activityRepository,IActivityGuideRepository activityGuideRepository,IUnitOfWork unitOfWork)
        {
            _IActivityRepository = activityRepository;
            _IActivityGuideRepository = activityGuideRepository;
            _IunitOfWork = unitOfWork;
        }

        public async Task<List<ActivityDto>> GetAllActivities(string category)
        {
            //指定分頁大小
            var now = DateTime.Now;
            var halfyearLater = now.AddMonths(6);

            var result = await _IActivityRepository.GetAllActivities();
            
            if (result == null || result.Count == 0) { return new List<ActivityDto>(); }

            var query = result.Where(a => a.StartTime >= now && a.StartTime <= halfyearLater);

            if(!string.IsNullOrEmpty(category) && category != "所有活動")
            {
                query = query.Where(activity =>
                    activity.Tags.Any(tag =>
                        tag.TagCategory != null &&
                        tag.TagCategory.TagCategory1 == category
                    )
                );
            }
            return query
                .Select(a => new ActivityDto
            {
                ActivityId = a.ActivityId,
                Title = a.Title,
                Description = a.Description,
                Location = a.Location,
                StartTime = a.StartTime,
                ActivityImg = UrlHelper.ImgFullUrl(a.ActivityImg,"events"),
            }).
            ToList();
        }

        public async Task<ActivityInfoDto> GetActivityInfo(int activityId)
        {
            var result = await _IActivityRepository.GetActivityInfo(activityId);
            if (result == null)
            {
                return null;
            }
            {
                return new ActivityInfoDto
                {
                    MaxCapacity = result.MaxCapacity,
                    OrganizerName = result.OrganizerName,
                    RegistrationEndTime = result.RegistrationEndTime,
                    Location = result.Location,
                    StartTime = result.StartTime,
                };
            }
        }
        public async Task<ActivityMapDto> GetActivityMap(int activityId)
        {
            var result = await _IActivityRepository.GetActivityMap(activityId);
            if (result == null)
            {
                return null;
            }
            {
                return new ActivityMapDto
                {
                    Title = result.Title,
                    Location = result.Location,
                    Latitude = result.Latitude,
                    Longitude = result.Longitude,
                };
            }
        }


        public async Task CreateActivities(int memberId, ActivityApplyDto applydto, ActivityGuideDto guidedto)
        {

            //建entity
            var activity = new Activity
            {
                //抓[authorize]傳入之id
                MemberId = memberId,
                Title = applydto.Title,
                Description = applydto.Description,
                Location = applydto.Location,
                StartTime = applydto.StartTime,
                EndTime = applydto.EndTime,
                MaxCapacity = applydto.MaxCapacity,
                RegistrationStartTime = applydto.RegistrationStartTime,
                RegistrationEndTime = applydto.RegistrationEndTime,
                Latitude = applydto.Latitude,
                Longitude = applydto.Longitude,
                OrganizerName = applydto.OrganizerName,
            };
            //直接建立物件關聯
            var guide = new ActivityGuide
            {
                ActivityEditorHtml = guidedto.ActivityEditorHtml,
                Activity = activity,
            };

            
            await _IActivityRepository.CreateActivityAsync(activity);

            await _IActivityGuideRepository.CreateActivityGuideAsync(guide);
            //統一存檔，若有一部分出現問題導致失敗也不會有存一半髒資料問題
            await _IunitOfWork.CompleteAsync();
        }

        
    }


}
