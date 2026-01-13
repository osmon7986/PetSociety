using System.ComponentModel.DataAnnotations;

namespace PetSociety.API.DTOs.User
{
    public class ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "密碼長度至少需 6 碼")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "新密碼與確認密碼不一致")]
        public string ConfirmNewPassword { get; set; }
    }
}