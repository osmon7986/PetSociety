using PetSociety.API.DTOs.Events;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Events.Interfaces;
using PetSociety.API.Services.Events.Interfaces;
using static PetSociety.API.Services.Events.ActivityCommentService;

namespace PetSociety.API.Services.Events
{
        public class ActivityCommentService : IActivityCommentService
        {
            private readonly IActivityCommentRepository _commentRepository;
            private readonly IContentModerator _contentModerator;

            public ActivityCommentService(
                IActivityCommentRepository commentRepository,
                IContentModerator contentModerator)
            {
                _commentRepository = commentRepository;
                _contentModerator = contentModerator;
            }

            public async Task<ActivityComment> AddCommentAsync(ActivityCommentDto dto)
            {
                // 1. 呼叫內容審核
                var (isFlagged,reason) = await _contentModerator.CheckAsync(dto.ActivityComment1);

                if (isFlagged)
                {
                    // 審核未通過，拋出異常，不進入 Repository
                    throw new InvalidOperationException("留言內容包含不當詞彙或未通過審核。");
                }

                // 2. 實體轉換 (Mapping)
                var activitycomment = new ActivityComment
                {
                    //tofo: MemberId 應串接
                    ActivityComment1 = dto.ActivityComment1,
                    ActivityId = dto.ActivityId,
                    MemberId = dto.MemberId,
                    CreateDate = DateTime.UtcNow
                };

                // 3. 呼叫 Repository 存檔
                await _commentRepository.AddAsync(activitycomment);

                return activitycomment;
            }
            public async Task<List<GetActivityCommentDto>> GetActivityComments(int activityId)
            {
                return await _commentRepository.GetActivityComment(activityId);
            }
        

        }
}
