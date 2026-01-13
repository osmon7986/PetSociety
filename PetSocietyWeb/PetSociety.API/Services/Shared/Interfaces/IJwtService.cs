using PetSociety.API.DTOs.Shared;

namespace PetSociety.API.Services.Shared.Interfaces
{
    public interface IJwtService
    {
        // 接收 Claims 資料，返回 JWT 字串
        string GenerateToken(UserClaimsDTO userClaims);
    }
}
