using System.ComponentModel.DataAnnotations;

namespace PetSociety.API.DTOs.User
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}