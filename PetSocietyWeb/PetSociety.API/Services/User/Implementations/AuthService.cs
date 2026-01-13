using PetSociety.API.DTOs.User;
using PetSociety.API.Helpers;
using PetSociety.API.Models;
using PetSociety.API.Repositories.User.Interfaces;
using PetSociety.API.Services.User.Interfaces;
using Google.Apis.Auth;

namespace PetSociety.API.Services.User.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IMemberRepository _repo;

        public AuthService(IMemberRepository repo)
        {
            _repo = repo;
        }

        public async Task<MemberDto> RegisterAsync(RegisterDto dto)
        {
            if (await _repo.GetMemberByEmailAsync(dto.Email) != null)
            {
                throw new Exception("Email already exists");
            }

            var code = new Random().Next(100000, 999999).ToString();

            var member = new Member
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = PasswordHelper.HashPassword(dto.Password),
                RegistrationDate = DateTime.Now,
                LastloginDate = DateTime.Now,
                IsActive = false,
                VerificationCode = code,
                Role = 0
            };

            await _repo.CreateMemberAsync(member);

            try
            {
                var emailHelper = new EmailHelper();
                emailHelper.SendAccountVerificationEmail(dto.Email, code);
            }
            catch (Exception ex)
            {
                Console.WriteLine("驗證信發送失敗: " + ex.Message);
            }

            return new MemberDto {
                MemberId = member.MemberId,
                Name = member.Name,
                Email = member.Email,
                Role = member.Role,
                RegistrationDate = member.RegistrationDate,
                IsActive = member.IsActive
            };
        }

        public async Task<MemberDto?> LoginAsync(LoginDto dto)
        {
            var member = await _repo.GetMemberByEmailAsync(dto.Email);
            if (member == null) return null;

            if (member.Password != PasswordHelper.HashPassword(dto.Password)) return null;

            if (!member.IsActive)
            {
                if (!string.IsNullOrEmpty(member.VerificationCode))
                    throw new Exception("帳號尚未驗證，請先收取信件並啟用帳號");
                else
                    throw new Exception("此帳號已被管理員停權，請聯繫客服");
            }

            member.LastloginDate = DateTime.Now;
            await _repo.UpdateMemberAsync(member);

            return new MemberDto
            {
                MemberId = member.MemberId,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Role = member.Role,
                RegistrationDate = member.RegistrationDate, 
                LastLoginDate = member.LastloginDate,       
                IsActive = member.IsActive

            };
        }

        public async Task<MemberDto> GoogleLoginAsync(string googleIdToken)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { "972290926876-vr878fu52s36ln8pkuu0glp0c4toholf.apps.googleusercontent.com" }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(googleIdToken, settings);
            }
            catch (InvalidJwtException)
            {
                throw new Exception("無效的 Google Token");
            }

            var member = await _repo.GetMemberByEmailAsync(payload.Email);

            if (member == null)
            {
                byte[] profilePicBytes = null;
                try
                {
                    if (!string.IsNullOrEmpty(payload.Picture))
                    {
                        using (var httpClient = new HttpClient())
                        {
                            profilePicBytes = await httpClient.GetByteArrayAsync(payload.Picture);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Google 頭像下載失敗");
                }

                member = new Member
                {
                    Name = payload.Name,
                    Email = payload.Email,
                    Password = PasswordHelper.HashPassword(Guid.NewGuid().ToString()),
                    RegistrationDate = DateTime.Now,
                    IsActive = true,
                    VerificationCode = null,
                    Role = 0,
                    ProfilePic = profilePicBytes,
                    LastloginDate = DateTime.Now
                };

                await _repo.CreateMemberAsync(member);
            }
            else
            {
                member.LastloginDate = DateTime.Now;

                if (!member.IsActive)
                {
                    member.IsActive = true;
                    member.VerificationCode = null;
                }
                await _repo.UpdateMemberAsync(member);

            }

            return new MemberDto
            {
                MemberId = member.MemberId,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone, 
                Role = member.Role,
                RegistrationDate = member.RegistrationDate, 
                LastLoginDate = member.LastloginDate,       
                IsActive = member.IsActive

            };
        }

        public async Task VerifyAccountAsync(string email, string code)
        {
            var member = await _repo.GetMemberByEmailAsync(email);
            if (member == null) throw new Exception("找不到此 Email");

            if (member.IsActive) throw new Exception("帳號已經啟用過了，請直接登入");

            if (member.VerificationCode != code)
            {
                throw new Exception("驗證碼錯誤");
            }

            member.IsActive = true;
            member.VerificationCode = null;
            await _repo.UpdateMemberAsync(member);
        }

        public async Task<string> ForgotPasswordAsync(string email)
        {
            var member = await _repo.GetMemberByEmailAsync(email);
            if (member == null) throw new Exception("找不到此 Email");

            var token = new Random().Next(100000, 999999).ToString();

            member.PasswordResetToken = token;
            member.ResetTokenExpires = DateTime.Now.AddMinutes(10);
            await _repo.UpdateMemberAsync(member);

            try
            {
                var emailHelper = new EmailHelper();
                emailHelper.SendPasswordResetEmail(email, token);
            }
            catch (Exception ex)
            {
                throw new Exception("寄信失敗: " + ex.Message);
            }

            return "驗證信已寄出，請檢查您的信箱";
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var member = await _repo.GetMemberByEmailAsync(dto.Email);
            if (member == null) throw new Exception("找不到此 Email");

            if (member.PasswordResetToken != dto.Token)
                throw new Exception("驗證碼錯誤");

            if (member.ResetTokenExpires < DateTime.Now)
                throw new Exception("驗證碼已過期，請重新申請");

            member.Password = PasswordHelper.HashPassword(dto.NewPassword);
            member.PasswordResetToken = null;
            member.ResetTokenExpires = null;

            await _repo.UpdateMemberAsync(member);
        }
    }
}