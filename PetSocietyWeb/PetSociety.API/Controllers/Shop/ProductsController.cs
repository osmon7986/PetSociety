using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Shop;
using PetSociety.API.Models;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetSociety.API.Controllers.Shop
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly PetSocietyContext _petsContext;

        public ProductsController(PetSocietyContext petsContext)
        {
            _petsContext = petsContext;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<object>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 15,
            [FromQuery] string category = "全部商品", // 新增：接收分類參數
            [FromQuery] decimal? minPrice = null, //  新增：最低價
            [FromQuery] decimal? maxPrice = null, //  新增：最高價
            [FromQuery] string? keyword = null) 
        {
            // ★ 修改 ：在這裡定義圖片所在的網站網址 (MVC 網站的 Port)
            string webUrl = "https://localhost:7032";

            // 準備 SQL (還沒執行) 
            // 這裡只做資料庫聽得懂的事：篩選、排序、關聯
            var query = _petsContext.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.Status == "上架中");

            // 新增：分類篩選邏輯 (一定要在分頁之前做！)
            if (!string.IsNullOrEmpty(category) && category != "全部商品")
            {
                query = query.Where(p => p.Category.CategoryName == category);
            }

            // 新增：價格篩選 (可以跟分類同時存在！) 
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                // 搜尋商品名稱 (也可以順便搜尋描述 p.Description)
                query = query.Where(p => p.ProductName.Contains(keyword));
            }

            // 先計算「符合條件的總筆數」(這時候 query 已經包含分類條件了)
            var totalCount = await query.CountAsync();

            // 加上分頁邏輯 (Skip & Take)
            // Skip: 跳過前面 (page - 1) * pageSize 筆
            // Take: 接著拿 pageSize 筆
            var products = await query
                // 第一順位：庫存少於 20 的 (HOT) 排最前面
                // (原理：True 比 False 大，所以 Descending 會把 True 排前面)
                .OrderByDescending(p => p.Stock < 20)

                // 第二順位：如果庫存狀況一樣，就比誰比較新 (NEW)
                .ThenByDescending(p => p.CreateDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 加工處理 (在記憶體執行) 
            var result = products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price,
                Stock = p.Stock,
                CategoryName = p.Category.CategoryName,
                Description = p.Description,
                CreateDate = p.CreateDate,

                // 處理畫面用的縮圖 (取第1張)
                // ★ 修改：縮圖 (Thumbnail) 改成回傳網址
                Thumbnail = p.ProductImages.OrderBy(img => img.ImageId).FirstOrDefault()?.ImagePath != null
                    ? $"{webUrl}/{p.ProductImages.OrderBy(img => img.ImageId).First().ImagePath}"
                    : null,

                // 處理彈窗用的圖片清單 (取全部)
                Images = p.ProductImages
                    .Where(img => !string.IsNullOrEmpty(img.ImagePath))
                    .OrderBy(img => img.ImageId)
                    .Select(img => $"{webUrl}/{img.ImagePath}")
                    .ToList()
            }).ToList();

            // 回傳一個物件，包含總數和資料列表
            return Ok(new
            {
                TotalCount = totalCount, // 告訴前端總共有幾筆
                Items = result           // 這一頁的 15 筆商品
            });

        }
        // GET: api/Products/5 (單一商品明細)
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProduct(int id)
        {
            string webUrl = "https://localhost:7032";
            var product = await _petsContext.Products
                .Include(p => p.Category)
                .Include(p => p.Member)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            // ★ 修改：把路徑轉成完整網址清單
            var imageUrls = product.ProductImages
                 .Where(img => !string.IsNullOrEmpty(img.ImagePath))
                 .OrderBy(img => img.ImageId)
                 .Select(img => $"{webUrl}/{img.ImagePath}")
                 .ToList();

            var result = new
            {
                product.ProductId,
                product.ProductName,
                product.Description,
                product.Price,
                product.Stock,
                CategoryName = product.Category?.CategoryName,
                SellerEmail = product.Member?.Email,
                Images = imageUrls // 回傳圖片陣列
            };

            return Ok(result);
        }
    }
}
