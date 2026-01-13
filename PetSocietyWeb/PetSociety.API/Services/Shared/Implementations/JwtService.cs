using Microsoft.IdentityModel.Tokens;
using PetSociety.API.DTOs.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using PetSociety.API.Services.Shared.Interfaces;

namespace PetSociety.API.Services.Shared.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(UserClaimsDTO userClaims)
        {
            // 讀取設定檔數值
            var secret = _configuration["JwtSettings:Secret"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expireMinutes = int.Parse(_configuration["JwtSettings:ExpireMinutes"]);

            if (string.IsNullOrEmpty(secret)) throw new InvalidOperationException("JWT Secret is not configured.");

            // 建立 Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secret); // 將密鑰轉換成位元組陣列

            var tokenDescriptor = new SecurityTokenDescriptor // 定義 Token 描述
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userClaims.Id.ToString()),
                    new Claim(ClaimTypes.Name, userClaims.Username),
                    new Claim(ClaimTypes.Role, userClaims.Role),
                }),
                Issuer = issuer,                          // Token 發行者
                Audience = audience,                      // Token 受眾
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
                // 使用 HmacSha256 演算法進行簽章，確保Header/Payload沒有被惡意修改
                // JWT = Base64UrlEncode(Header) . Base64UrlEncode(Payload) . Signature
                // Signature = HMACSHA256(SecretKey, SignatureInput)
            };

            // 使用 tokenDescriptor 的定義建立 JWT 物件
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token); // 將 JWT 物件序列化成字串形式
        }
    }
}
