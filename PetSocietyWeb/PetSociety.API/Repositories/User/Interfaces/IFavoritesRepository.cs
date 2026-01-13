using PetSociety.API.Models;
using System.Threading.Tasks;

namespace PetSociety.API.Repositories.User.Interfaces
{
    public interface IFavoritesRepository
    {
        // 新增收藏
        Task AddAsync(Favorite favorite);

        // 根據會員ID和類型查詢
        Task<List<Favorite>> GetByMemberAndTypeAsync(int memberId, int targetType);

        // 檢查是否已收藏
        Task<bool> ExistsAsync(int memberId, int targetId, int targetType);

        // 刪除收藏
        Task DeleteAsync(int favoriteId);

        // 找單筆 (為了刪除前檢查權限)
        Task<Favorite?> GetByIdAsync(int favoriteId);

        Task<Favorite?> GetByTargetAsync(int memberId, int targetId, int targetType);
    }
}
