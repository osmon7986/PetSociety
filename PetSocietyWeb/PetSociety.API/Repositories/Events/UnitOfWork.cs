using PetSociety.API.Models;
using PetSociety.API.Repositories.Events.Interfaces;

namespace PetSociety.API.Repositories.Events
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly PetSocietyContext _context;

        public UnitOfWork(PetSocietyContext context) 
        {
            _context = context;
        }
        
        //實作CompletAsync 包裹SaveChangeAsync
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
