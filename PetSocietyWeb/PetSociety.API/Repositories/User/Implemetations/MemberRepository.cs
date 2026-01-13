using Microsoft.EntityFrameworkCore;
using PetSociety.API.Models;
using PetSociety.API.Repositories.User.Interfaces;

namespace PetSociety.API.Repositories.User.Implemetations
{
    public class MemberRepository : IMemberRepository
    {
        private readonly PetSocietyContext _context;

        public MemberRepository(PetSocietyContext context)
        {
            _context = context;
        }

        public async Task<Member?> GetMemberByEmailAsync(string email)
        {
            return await _context.Members
                             .FirstOrDefaultAsync(m => m.Email.ToLower() == email.ToLower());
        }

        public async Task CreateMemberAsync(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProfilePicAsync(int memberId, byte[] imageBytes)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member != null)
            {
                member.ProfilePic = imageBytes;
                await _context.SaveChangesAsync();
            }
        }


        public async Task<byte[]?> GetProfilePicAsync(int memberId)
        {
            var member = await _context.Members.FindAsync(memberId);
            return member?.ProfilePic;
        }

        public async Task<Member> GetByIdAsync(int id)
        {
            return await _context.Members.FindAsync(id);
        }

        public async Task UpdatePasswordAsync(Member member)
        {
            _context.Members.Update(member); 
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMemberAsync(Member member)
        {
            // Entity Framework 的 Update 方法會標記這個物件為"已修改"
            _context.Members.Update(member);

            await _context.SaveChangesAsync();
        }

    }
}