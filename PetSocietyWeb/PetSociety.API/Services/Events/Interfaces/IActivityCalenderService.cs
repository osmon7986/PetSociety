using PetSociety.API.DTOs.Events;

namespace PetSociety.API.Services.Events.Interfaces
{
    public interface IActivityCalenderService
    {
        Task<List<ActivityCalenderDto>> GetActivityCalenders(int? memberId);
    }
}
