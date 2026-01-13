namespace PetSociety.API.Services.Events.Interfaces
{
    public interface IContentModerator
    {
        Task<(bool IsFlagged, string Reason)> CheckAsync(string text);
    }
}
