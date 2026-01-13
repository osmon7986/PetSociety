using Microsoft.EntityFrameworkCore;
using PetSociety.API.Models;
using PetSociety.API.Repositories.User.Interfaces;

namespace PetSociety.API.Repositories.User.Implemetations
{
    public class FavoritesRepository : IFavoritesRepository
    {
        private readonly PetSocietyContext _context;

        public FavoritesRepository(PetSocietyContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Favorite favorite)
        {
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Favorite>> GetByMemberAndTypeAsync(int memberId, int targetType)
        {
            return await _context.Favorites
                .Where(f => f.MemberId == memberId && f.TargetType == targetType)
                .OrderByDescending(f => f.CreateDate) 
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int memberId, int targetId, int targetType)
        {
            return await _context.Favorites.AnyAsync(f =>
                f.MemberId == memberId &&
                f.TargetId == targetId &&
                f.TargetType == targetType);
        }

        public async Task DeleteAsync(int favoriteId)
        {
            var fav = await _context.Favorites.FindAsync(favoriteId);
            if (fav != null)
            {
                _context.Favorites.Remove(fav);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Favorite?> GetByIdAsync(int favoriteId)
        {
            return await _context.Favorites.FindAsync(favoriteId);
        }

        public async Task<Favorite?> GetByTargetAsync(int memberId, int targetId, int targetType)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f =>
                    f.MemberId == memberId &&
                    f.TargetId == targetId &&
                    f.TargetType == targetType);
        }
    }
}
