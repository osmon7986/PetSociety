using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PetSociety.API.Services.User.Interfaces;

namespace PetSociety.API.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class AvatarController : ControllerBase
    {
        private readonly IMemberService _service;

        public AvatarController(IMemberService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile file) 
        {
            var memberIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(memberIdStr) || !int.TryParse(memberIdStr, out int memberId))
            {
                return Unauthorized("無法識別使用者身分");
            }

            if (file == null || file.Length == 0)
                return BadRequest("請選擇檔案");

            if (file.Length > 2 * 1024 * 1024)
                return BadRequest("檔案太大，請小於 2MB");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("只支援 JPG, PNG, GIF 格式的圖片");
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();

                    await _service.UpdateProfilePicAsync(memberId, fileBytes);
                }

                return Ok(new { message = "頭像上傳成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"內部錯誤: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvatar() 
        {
            var memberIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(memberIdStr) || !int.TryParse(memberIdStr, out int memberId))
            {
                return Unauthorized();
            }

            var imageBytes = await _service.GetProfilePicAsync(memberId);

            if (imageBytes == null || imageBytes.Length == 0)
            {
                return NotFound("尚未上傳頭像");
            }

            string mimeType = GetImageMimeType(imageBytes);
            return File(imageBytes, mimeType);
        }

        private string GetImageMimeType(byte[] bytes)
        {
            if (bytes.Length < 4) return "image/jpeg";
            if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47) return "image/png";
            if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46) return "image/gif";
            if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF) return "image/jpeg";
            return "image/jpeg";
        }
    }
}