using PetSociety.API.DTOs.User;

namespace PetSociety.API.Services.User.Interfaces
{
    public interface IAuthService
    {
        Task<MemberDto> RegisterAsync(RegisterDto dto);

        Task<MemberDto?> LoginAsync(LoginDto dto);

        Task<MemberDto> GoogleLoginAsync(string googleIdToken);

        Task VerifyAccountAsync(string email, string code);
        Task<string> ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordDto dto);
    }
}
