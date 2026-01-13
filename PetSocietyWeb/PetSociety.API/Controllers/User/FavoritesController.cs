using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetSociety.API.DTOs.User;
using PetSociety.API.Services.User.Interfaces;
using System.Security.Claims; 
using System.Threading.Tasks;

namespace PetSociety.API.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoritesService _service;

        public FavoritesController(IFavoritesService service)
        {
            _service = service;
        }

        // 1. 新增收藏
        // POST: api/Favorites
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddFavoriteDto dto)
        {
            // 從 JWT Token 取得 MemberId
            var memberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var success = await _service.AddFavoriteAsync(memberId, dto);
            if (!success)
            {
                return BadRequest(new { message = "已經收藏過此項目" });
            }

            return Ok(new { message = "收藏成功" });
        }

        // 2. 取得收藏列表
        // GET: api/Favorites?targetType=1
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int targetType)
        {
            var memberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var list = await _service.GetFavoritesAsync(memberId, targetType);
            return Ok(list);
        }

        // 3. 移除收藏
        // DELETE: api/Favorites/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var memberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var success = await _service.RemoveFavoriteAsync(memberId, id);
            if (!success)
            {
                return NotFound(new { message = "找不到此收藏或無權限刪除" });
            }

            return Ok(new { message = "已移除收藏" });
        }

        // DELETE: api/Favorites/delete-by-target?targetId=20&targetType=1
        [HttpDelete("delete-by-target")]
        public async Task<IActionResult> DeleteByTarget([FromQuery] int targetId, [FromQuery] int targetType)
        {
            var memberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var success = await _service.RemoveFavoriteByTargetAsync(memberId, targetId, targetType);

            if (!success)
            {
                return NotFound(new { message = "找不到此收藏" });
            }

            return Ok(new { message = "已移除收藏" });
        }
    }
}
