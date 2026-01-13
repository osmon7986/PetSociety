using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using PetSociety.API.DTOs.User;
using PetSociety.API.Helpers;
using PetSociety.API.Models;
using PetSociety.API.Repositories.User.Interfaces;
using PetSociety.API.Services.User.Interfaces;
using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;
using System.Net.Http;


namespace PetSociety.API.Services.User.Implementations
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _repo;

        public MemberService(IMemberRepository repo)
        {
            _repo = repo;
        }

        public async Task<MemberDto> GetMemberAsync(int id)
        {
            var member = await _repo.GetByIdAsync(id);

            if (member == null) return null;

            return new MemberDto
            {
                MemberId = member.MemberId,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Role = member.Role,
                RegistrationDate = member.RegistrationDate,
                LastLoginDate = member.LastloginDate
            };
        }

        public async Task UpdateMemberAsync(int memberId, UpdateProfileDto request)
        {
            var member = await _repo.GetByIdAsync(memberId);

            if (member == null)
            {
                throw new Exception("找不到該會員");
            }

            // 注意：這裡只更新前端允許修改的欄位，Email 通常不讓改，密碼有專屬功能改
            member.Name = request.Name;
            member.Phone = request.Phone;

            await _repo.UpdateMemberAsync(member);
        }
        public async Task UpdateProfilePicAsync(int memberId, byte[] imageBytes)
        {
            await _repo.UpdateProfilePicAsync(memberId, imageBytes);
        }

        public async Task<byte[]?> GetProfilePicAsync(int memberId)
        {
            return await _repo.GetProfilePicAsync(memberId);
        }

        public async Task ChangePasswordAsync(int memberId, ChangePasswordDto dto)
        {
            var member = await _repo.GetByIdAsync(memberId);
            if (member == null) throw new Exception("找不到該會員");

            var oldHash = PasswordHelper.HashPassword(dto.OldPassword);
            if (member.Password != oldHash)
            {
                throw new Exception("舊密碼錯誤");
            }

            member.Password = PasswordHelper.HashPassword(dto.NewPassword);

            await _repo.UpdateMemberAsync(member);
        }
   
    }
}