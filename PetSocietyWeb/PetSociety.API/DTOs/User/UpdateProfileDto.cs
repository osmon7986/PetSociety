namespace PetSociety.API.DTOs.User
{
    public class UpdateProfileDto
    {
        // 這裡列出允許使用者修改的欄位
        public string Name { get; set; }
        public string Phone { get; set; }
     
    }
}