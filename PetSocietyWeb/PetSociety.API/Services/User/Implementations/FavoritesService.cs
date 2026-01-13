using PetSociety.API.DTOs.User;
using PetSociety.API.Models;
using PetSociety.API.Repositories.User.Interfaces;
using PetSociety.API.Services.User.Interfaces;

namespace PetSociety.API.Services.User.Implementations
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IFavoritesRepository _repo;

        public FavoritesService(IFavoritesRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> AddFavoriteAsync(int memberId, AddFavoriteDto dto)
        {
            // 1. 檢查是否重複收藏
            if (await _repo.ExistsAsync(memberId, dto.TargetId, dto.TargetType))
            {
                return false; 
            }

            // 2. 轉成 Entity
            var favorite = new Favorite
            {
                MemberId = memberId,
                TargetId = dto.TargetId,
                TargetType = dto.TargetType,
                Title = dto.Title,       
                ImageUrl = dto.ImageUrl, 
                Price = dto.Price,
                Intro = dto.Intro,
                CreateDate = DateTime.Now
            };

            // 3. 存入 DB
            await _repo.AddAsync(favorite);
            return true;
        }

        public async Task<List<FavoriteItemDto>> GetFavoritesAsync(int memberId, int targetType)
        {
            var list = await _repo.GetByMemberAndTypeAsync(memberId, targetType);

            return list.Select(f => new FavoriteItemDto
            {
                FavoriteId = f.FavoriteId,
                TargetId = f.TargetId,
                TargetType = f.TargetType,
                Title = f.Title,
                ImageUrl = f.ImageUrl,
                Price = f.Price,
                Intro = f.Intro,
                CreateDate = f.CreateDate.ToString("yyyy-MM-dd")
            }).ToList();
        }

        public async Task<bool> RemoveFavoriteAsync(int memberId, int favoriteId)
        {
            var fav = await _repo.GetByIdAsync(favoriteId);
            if (fav == null || fav.MemberId != memberId)
            {
                return false;
            }

            await _repo.DeleteAsync(favoriteId);
            return true;
        }
        public async Task<bool> RemoveFavoriteByTargetAsync(int memberId, int targetId, int targetType)
        {
            var favorite = await _repo.GetByTargetAsync(memberId, targetId, targetType);

            if (favorite == null)
            {
                return false; 
            }
            await _repo.DeleteAsync(favorite.FavoriteId);
            return true;
        }
    }
}
