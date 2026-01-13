using PetSociety.API.DTOs.User;
using System.Threading.Tasks;

namespace PetSociety.API.Services.User.Interfaces
{
    public interface IMemberService
    {
        Task<MemberDto> GetMemberAsync(int id);
        Task UpdateMemberAsync(int memberId, UpdateProfileDto request);
        Task ChangePasswordAsync(int memberId, ChangePasswordDto dto);

        Task UpdateProfilePicAsync(int memberId, byte[] imageBytes);
        Task<byte[]?> GetProfilePicAsync(int memberId);

    }
}