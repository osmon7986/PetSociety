using PetSociety.API.DTOs.Events;
using PetSociety.API.Models;

namespace PetSociety.API.Services.Events.Interfaces
{
    public interface IActivityCommentService
    {
        Task<ActivityComment> AddCommentAsync(ActivityCommentDto dto);

        Task<List<GetActivityCommentDto>> GetActivityComments(int activityId);
    }
}
