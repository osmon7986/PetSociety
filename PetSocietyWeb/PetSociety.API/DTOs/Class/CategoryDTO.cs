namespace PetSociety.API.DTOs.Class
{
    public record CategoryDTO(int Id, string Name);
    // record: C# 9 新類別
    // C# 會自動產生屬性和建構子
    // 預設唯獨
    // 用內容比較，只要值(value)一樣就相等
}
