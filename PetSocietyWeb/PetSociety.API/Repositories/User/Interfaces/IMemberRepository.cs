using PetSociety.API.Models; // 引用你的資料庫模型
using System.Threading.Tasks;

namespace PetSociety.API.Repositories.User.Interfaces
{
    public interface IMemberRepository
    {
        // 定義：透過 Email 找會員 (用於檢查是否重複註冊，或登入時抓資料)
        Task<Member?> GetMemberByEmailAsync(string email);

        // 定義：新增會員 (註冊用)
        Task CreateMemberAsync(Member member);

        // 未來如果有其他功能 (如修改密碼)，就繼續加在下面...
        // Task UpdateMemberAsync(Member member);

        Task UpdateProfilePicAsync(int memberId, byte[] imageBytes);
        Task<byte[]?> GetProfilePicAsync(int memberId);

        Task<Member> GetByIdAsync(int id);
        Task UpdateMemberAsync(Member member);
    }
}