using PetSociety.API.DTOs.User;

namespace PetSociety.API.Services.User.Interfaces
{
    public interface IFavoritesService
    {
        Task<bool> AddFavoriteAsync(int memberId, AddFavoriteDto dto);
        Task<List<FavoriteItemDto>> GetFavoritesAsync(int memberId, int targetType);
        Task<bool> RemoveFavoriteAsync(int memberId, int favoriteId);
        Task<bool> RemoveFavoriteByTargetAsync(int memberId, int targetId, int targetType);
    }
}
