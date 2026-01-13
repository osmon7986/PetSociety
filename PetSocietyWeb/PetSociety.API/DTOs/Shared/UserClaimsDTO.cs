namespace PetSociety.API.DTOs.Shared
{
    public class UserClaimsDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }
}
