namespace PetSociety.API.DTOs.Shared
{
    // 登入成功後返回給前端的資料格式
    public class TokenDTO
    {
        public string Token { get; set; } = string.Empty;
        public int ExpireIn { get; set; } // Token 有效期限 (例如：秒)
        public string TokenType { get; set; } = "Bearer";
        
    }
}
