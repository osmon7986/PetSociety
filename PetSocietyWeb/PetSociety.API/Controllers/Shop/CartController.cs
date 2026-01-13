using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Shop;
using PetSociety.API.Extensions;
using PetSociety.API.Models;
using System.Security.Claims; // 這是解 Token 必備的
using System.Threading.Tasks;
using static Betalgo.Ranul.OpenAI.ObjectModels.RealtimeModels.RealtimeEventTypes.Client.Conversation;


namespace PetSociety.API.Controllers.Shop
{
    // 網址規則：[controller] 會自動換成類別名稱 "Cart"
    // 所以最終網址是：api/Cart
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // ★ 重點：加上這行，沒帶 Token 的人連門都進不來
    public class CartController : ControllerBase
    {
        private readonly PetSocietyContext _context;
        public CartController(PetSocietyContext context)
        {
            _context = context;
        }

        [HttpPost("add")] // 最終網址：POST api/Cart/add
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto request)
        {
            // 從 Token 裡面抓出「是誰登入的」
            // User 是 Controller 內建的變數，只要通過 [Authorize]，裡面就會有資料
            int memberId = User.GetMemberId();

            // 檢查該會員購物車是否已有該商品 (避免重複加入)
            var existingItem = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.MemberId == memberId && c.ProductId == request.ProductId);
            if (existingItem != null)
            {
                // 如果已經有了，就加數量
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                // 如果沒有，就新增一筆
                var newItem = new ShoppingCart
                {
                    MemberId = memberId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    CreateDate = DateTime.Now
                };
                _context.ShoppingCarts.Add(newItem);
            }

            // ★ 最重要的一步：存檔！
            await _context.SaveChangesAsync();

            return Ok(new { message = "加入成功" });
        }

        // GET: api/Cart/count
        [HttpGet("count")]
        public async Task<IActionResult> GetCartCount()
        {
            // 1. 抓使用者 ID
            int memberId = User.GetMemberId();
            // 2. 統計總數量 (Sum)
            // 這裡會把購物車裡每一筆的 Quantity 加總起來
            var totalCount = await _context.ShoppingCarts
                .Where(c => c.MemberId == memberId)
                .SumAsync(c => c.Quantity);

            return Ok(new { count = totalCount });
        }

        // GET: api/Cart/items
        [HttpGet("items")]
        public async Task<IActionResult> GetCartItems()
        {
            string webUrl = "https://localhost:7032";
            int memberId = User.GetMemberId();
            // 先撈資料 (這時候圖片還是 byte[] 二進位檔)
            var rawData = await _context.ShoppingCarts
                .Include(c => c.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(c =>c.MemberId == memberId)
                .Select(c => new
                {
                    c.ProductId,
                    c.Product.ProductName,
                    c.Product.Price,
                    c.Quantity,
                    // 這裡只撈「路徑字串」，資料庫看得懂！
                    FirstImagePath = c.Product.ProductImages
                      .OrderBy(img => img.ImageId)
                      .Select(img => img.ImagePath)
                      .FirstOrDefault()
                })
                .ToListAsync();

            var cartItems = rawData.Select(item => new CartItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity,

                // ★ 關鍵：在這裡組裝網址
                Thumbnail = !string.IsNullOrEmpty(item.FirstImagePath)
                  ? $"{webUrl}/{item.FirstImagePath}"
                  : null
            }).ToList();
            return Ok(cartItems);
        }

        // DELETE: api/Cart/items/{productId}
        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            int memberId = User.GetMemberId();

            // 找到要刪除的那一筆資料
            var cartItem = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.MemberId == memberId && c.ProductId == productId);

            // 如果找不到 (可能已經刪過了)，就回傳 NotFound
            if (cartItem == null) 
            {
                return NotFound(new { message = "購物車找不到此商品" });
            }

            _context.ShoppingCarts .Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new {message = "已移除商品"});
        }

        // PATCH: api/Cart/items/{productId}
        // 用來更新商品數量
        [HttpPatch("items/{productId}")]
        public async Task<IActionResult> UpdateCartItem(int productId, [FromBody] int newQuantity)
        {
            // 驗證數量 (至少要 1 個)
            if (newQuantity < 1)
            {
                return BadRequest(new
                {
                    message = "商品數量至少為1"
                });
            }

            int memberId = User.GetMemberId();

            // 找到該筆購物車資料
            var cartItem = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.MemberId == memberId && c.ProductId == productId);

            if (cartItem == null) 
            {
                return NotFound(new
                {
                    message = "找不到該商品"
                });
            }

            cartItem.Quantity = newQuantity;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "數量已更新" , currentQuantity = cartItem.Quantity
            });
        }
    }
}
