using PetSociety.API.DTOs.Events;
using PetSociety.API.Models;

namespace PetSociety.API.Repositories.Events.Interfaces
{
    public interface IActivityCommentRepository
    {
        Task AddAsync(ActivityComment activitycomment);

        Task<List<GetActivityCommentDto>> GetActivityComment(int activityId);
    }
}
