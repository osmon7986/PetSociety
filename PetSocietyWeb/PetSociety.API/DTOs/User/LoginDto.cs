using System.Text.Json.Serialization; 

namespace PetSociety.API.DTOs.User
{
    public class LoginDto
    {
       
        [JsonPropertyName("username")]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}