using Betalgo.Ranul.OpenAI.Interfaces;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Betalgo.Ranul.OpenAI.ObjectModels;
using PetSociety.API.Services.Events.Interfaces;
using System.Reflection; // 記得加這個，為了讀取 Categories

namespace PetSociety.API.Services.Events
{
    public class ContentModerator : IContentModerator
    {
        private readonly IOpenAIService _sdk;

        public ContentModerator(IOpenAIService sdk)
        {
            _sdk = sdk;
        }

        public async Task<(bool IsFlagged, string Reason)> CheckAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return (false, "Empty Input");
            }
            var request = new CreateModerationRequest
            {
                Input = text,
                Model = "omni-moderation-latest"
            };

            var result = await _sdk.Moderation.CreateModeration(request);



            if (result.Successful)
            {
                var firstResult = result.Results.FirstOrDefault();
                if (firstResult != null && firstResult.Flagged)
                {
                    // 反射邏輯維持不變
                    var violatedCategories = firstResult.Categories.GetType()
                        .GetProperties()
                        .Where(p => p.PropertyType == typeof(bool) && (bool)p.GetValue(firstResult.Categories) == true)
                        .Select(p => p.Name);

                    string reasonString = string.Join(", ", violatedCategories);
                    if (string.IsNullOrEmpty(reasonString)) reasonString = "違反社群守則";

                    return (true, $"不當內容: {reasonString}");
                }
                if (result.Error != null)
                {
                    throw new Exception($"OpenAI API 連線失敗: {result.Error.Code} - {result.Error.Message}");
                }
            }

            return (false, $"API Error: {result.Error?.Message}");
        }
    }
}
