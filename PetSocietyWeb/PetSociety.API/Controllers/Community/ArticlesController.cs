using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Community;
using PetSociety.API.Models;

namespace PetSociety.API.Controllers.Community
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly PetSocietyContext _petSocietyContext;

        public ArticlesController(PetSocietyContext petSocietyContext)
        {
            _petSocietyContext = petSocietyContext;
        }
        
        // GET: api/Articles?page=1&pageSize=10
        // 優化後：支援分頁查詢
        [HttpGet]
        public async Task<ActionResult<object>> GetArticles(int page = 1, int pageSize = 5, int? categoryId = null, int? tagId = null,string? keyword = null, int? memberId = null)
        {
            var query = _petSocietyContext.Articles
        .Include(a => a.Category) // 確保有 Include 關聯資料
        .Include(a => a.Tag)
        .Include(a => a.Member)   // 假設你有 Member 關聯
        .Include(a => a.FavoriteArticles)
        .AsQueryable();

            // 分類篩選邏輯
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }
            // 標籤篩選邏輯
            if (tagId.HasValue && tagId.Value > 0)
            {
                query = query.Where(a => a.TagId == tagId.Value);
            }
            //關鍵字篩選邏輯(搜尋標題或內容)
            if (!string.IsNullOrWhiteSpace(keyword))
            {   // 同時搜尋標題或內容包含關鍵字的文章
                query = query.Where(a => a.Title.Contains(keyword) || a.Content.Contains(keyword));
            }

            // 取得總筆數，供前端分頁元件使用
            var totalCount = await query.CountAsync();

            // 1. 先從資料庫取出原始資料
            var items = await query
                .OrderByDescending(x => x.PostDate) // 最新的文章排在前面
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ArticleDTO
                {
                    ArticleId = x.ArticleId,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.CategoryName : "未分類",
                    TagName = x.Tag != null ? x.Tag.TagName : "無標籤",
                    MemberName = x.Member != null ? x.Member.Name : "匿名會員",
                    Title = x.Title,
                    Content = x.Content,
                    PostDate = x.PostDate,
                    LastUpdate = x.LastUpdate,
                    CommentCount = x.CommentCount ?? 0,
                    Like = x.Like ?? 0,
                    DisLike = x.DisLike ?? 0,
                    Popular = x.Popular,
                    Picture = x.Picture,
                    IsFavorited = memberId.HasValue && x.FavoriteArticles.Any(f => f.MemberId == memberId.Value)
                })
                .ToListAsync();

            // 2. 在記憶體中進行 C# 運算 (解決 InvalidOperationException)
            foreach (var item in items)
            {
                // 呼叫靜態方法計算並轉為 int
                item.Popular = (int)Math.Round(CalculatePopular(item.Like, item.DisLike, item.CommentCount));
            }

            return Ok(new
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                Items = items
            });
        }

        // GET: api/Articles/Members
        [HttpGet("Members")]
        public async Task<ActionResult<IEnumerable<object>>> GetMembersList()
        {
            return await _petSocietyContext.Members
                .Select(m => new {
                    m.MemberId,
                    m.Name
                })
                .ToListAsync();
        }

        // POST: api/Articles/5/like
        // 按讚功能
        [HttpPost("{id}/like")]
        public async Task<IActionResult> LikeArticle(int id, [FromBody] VoteRequest request)
        {
            var article = await _petSocietyContext.Articles.FindAsync(id);
            if (article == null) return NotFound();

            if (request.Action == "add")
            {
                article.Like = (article.Like ?? 0) + 1;
            }
            else if (request.Action == "remove")
            {
                article.Like = Math.Max(0, (article.Like ?? 0) - 1);
            }
            else if (request.Action == "switch")
            {
                article.Like = (article.Like ?? 0) + 1;
                article.DisLike = Math.Max(0, (article.DisLike ?? 0) - 1); 
            }

            // 重新計算人氣
            article.Popular = (int)Math.Round(CalculatePopular(article.Like, article.DisLike, article.CommentCount));
            await _petSocietyContext.SaveChangesAsync();

            // 建議回傳完整的 Like 和 DisLike，讓前端同步
            return Ok(new
            {
                article.ArticleId,
                article.Like,
                article.DisLike,
                article.Popular
            });
        }
            // POST: api/Articles/5/dislike
            // 倒讚功能
            [HttpPost("{id}/dislike")]
        public async Task<IActionResult> DislikeArticle(int id, [FromBody] VoteRequest request)
        {
            var article = await _petSocietyContext.Articles.FindAsync(id);
            if (article == null) return NotFound();

            if (request.Action == "add")
            {
                article.DisLike = (article.DisLike ?? 0) + 1;
            }
            else if (request.Action == "remove")
            {
                article.DisLike = Math.Max(0, (article.DisLike ?? 0) - 1);
            }
            else if (request.Action == "switch")
            {
                article.DisLike = (article.DisLike ?? 0) + 1;
                article.Like = Math.Max(0, (article.Like ?? 0) - 1);
            }

            article.Popular = (int)Math.Round(CalculatePopular(article.Like, article.DisLike, article.CommentCount));
            await _petSocietyContext.SaveChangesAsync();

            return Ok(new { article.ArticleId, article.Like, article.DisLike, article.Popular });
        }

        public class VoteRequest
        {
            public string Action { get; set; } // "add", "remove", "switch"
        }

        // 私有方法：同步前端的 Popular 計算公式
        private static double CalculatePopular(int? like, int? dislike, int? commentCount)
        {
            int l = like ?? 0;
            int d = dislike ?? 0;
            int c = commentCount ?? 0;
            //int totalVotes = l + d;

            //double score = totalVotes == 0 ? 0 : (double)(l - d) / totalVotes;
            //double engagementBonus = Math.Log10(totalVotes + c + 1) * 0.1;
            double score = (l * 2) + (c * 3) - (d * 2);

            return Math.Max(0, score);
        }

        

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDTO>> GetArticle(int id,int? memberId = null)
        {
            var article = await _petSocietyContext.Articles
        .Include(a => a.Category)
        .Include(a => a.Tag)
        .Include(a => a.Member)
        .Include(a => a.FavoriteArticles)
        .FirstOrDefaultAsync(a => a.ArticleId == id);

            //var article = await _petSocietyContext.Articles.FindAsync(id);
            if (article == null) return NotFound();

            return new ArticleDTO
            {
                ArticleId = article.ArticleId,
                CategoryId = article.CategoryId,
                CategoryName = article.Category?.CategoryName ?? "未分類",
                TagId = article.TagId,
                TagName = article.Tag?.TagName ?? "無標籤",
                MemberId = article.MemberId,
                MemberName = article.Member?.Name ?? "匿名會員",
                Title = article.Title,
                Content = article.Content,
                PostDate = article.PostDate,
                LastUpdate = article.LastUpdate,
                Popular = article.Popular,
                Like = article.Like ?? 0,
                DisLike = article.DisLike ?? 0,
                CommentCount = article.CommentCount ?? 0,
                Picture = article.Picture,

                //這行一定要加，否則前端 Modal 裡的收藏狀態會錯
                IsFavorited = memberId.HasValue && article.FavoriteArticles.Any(f => f.MemberId == memberId.Value)
            };
        }

        [HttpPost]
        public async Task<ActionResult<ArticleDTO>> PostArticle(ArticleDTO articleDto)
        {
            var article = new Article
            {
                ArticleId = articleDto.ArticleId,
                CategoryId = articleDto.CategoryId,
                TagId = articleDto.TagId,
                MemberId = articleDto.MemberId,
                Title = articleDto.Title,
                Content = articleDto.Content,
                PostDate = DateTime.Now,
                LastUpdate = DateTime.Now,
                Like = 0,
                DisLike = 0,
                Popular = 0,
                CommentCount = 0,
                Picture = articleDto.Picture
            };
            _petSocietyContext.Articles.Add(article);
            await _petSocietyContext.SaveChangesAsync();

            articleDto.ArticleId = article.ArticleId;
            return CreatedAtAction(nameof(GetArticle), new { id = article.ArticleId }, articleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, ArticleDTO articleDto)
        {
            if (id != articleDto.ArticleId) return BadRequest();

            // 強制檢查：因為你現在只有 2 種分類 (ID: 1, 2)
            //if (articleDto.CategoryId < 1 || articleDto.CategoryId > 2)
            //{
            //    return BadRequest("分類 ID 錯誤！目前系統僅支援分類 狗 或 貓。");
            //}

            var article = await _petSocietyContext.Articles.FindAsync(id);
            if (article == null) return NotFound();

            article.Title = articleDto.Title;
            article.Content = articleDto.Content;
            article.CategoryId = articleDto.CategoryId;
            article.TagId = articleDto.TagId;
            article.Picture = articleDto.Picture;
            article.LastUpdate = DateTime.Now;

            _petSocietyContext.Entry(article).State = EntityState.Modified;

            try 
            { 
                await _petSocietyContext.SaveChangesAsync(); 
            }
            catch (DbUpdateException ex)
            {
                // 捕捉 SQL 衝突並回傳易讀訊息
                var innerException = ex.InnerException?.Message;
                return BadRequest($"資料更新失敗。細節：{innerException}");
            }
            return NoContent();
        }

        // 文章刪除
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _petSocietyContext.Articles.FindAsync(id);
            //  如果沒有文章，回傳null
            if (article == null) return NotFound();

            //  手動刪除所有該文章的留言
            //  這樣就算資料庫沒設「串聯刪除」，我們也先手動清空了
            var relatedComments = _petSocietyContext.Comments.Where(c => c.ArticleId == id);
            //  先刪留言
            _petSocietyContext.Comments.RemoveRange(relatedComments);
            //  刪除收藏記錄
            var relatedFavorites = _petSocietyContext.FavoriteArticles.Where(f => f.ArticleId == id);
            _petSocietyContext.FavoriteArticles.RemoveRange(relatedFavorites);
            //  再刪文章
            _petSocietyContext.Articles.Remove(article);
            //  一併送交資料庫執行
            await _petSocietyContext.SaveChangesAsync();

            return NoContent();
        }

        // 文章收藏
        [HttpPost("{id}/FavoriteArticle/{memberId}")]
        public async Task<IActionResult> ToggleFavoriteArticles([FromRoute] int id, [FromRoute] int memberId)
        {
            var existing = await _petSocietyContext.FavoriteArticles
                .FirstOrDefaultAsync(f => f.ArticleId == id && f.MemberId == memberId);

            if (existing != null)
            {
                _petSocietyContext.FavoriteArticles.Remove(existing);
                await _petSocietyContext.SaveChangesAsync();
                return Ok(new { isFavorited = false });
            }
            else
            {
                _petSocietyContext.FavoriteArticles.Add(new FavoriteArticle
                {
                    ArticleId = id,
                    MemberId = memberId,
                    CreatedDate = DateTime.Now
                });
                await _petSocietyContext.SaveChangesAsync();
                return Ok(new { isFavorited = true });
            }
        }

        // 取得該會員收藏的所有文章
        // GET: api/Articles/FavoriteArticles/20
        [HttpGet("FavoriteArticles/{memberId}")]
        public async Task<ActionResult<IEnumerable<ArticleDTO>>> GetUserFavoriteArticles(int memberId)
        {
            var favoritesArticles = await _petSocietyContext.FavoriteArticles
                .Where(f => f.MemberId == memberId)
                .Include(f => f.Article)
                    .ThenInclude(a => a.Category)
                .Include(f => f.Article)
                    .ThenInclude(a => a.Tag)
                .Include(f => f.Article)
                    .ThenInclude(a => a.Member)
                .OrderByDescending(f => f.CreatedDate) // 按收藏時間排序，最新的在前
                .ToListAsync();
                var results = favoritesArticles.Select(f => new ArticleDTO
                {
                    ArticleId = f.Article.ArticleId,
                    Title = f.Article.Title,
                    Content = f.Article.Content,
                    CategoryName = f.Article.Category?.CategoryName ?? "未分類",
                    TagName = f.Article.Tag?.TagName ?? "無標籤",
                    MemberName = f.Article.Member?.Name ?? "匿名會員",
                    PostDate = f.Article.PostDate,
                    Like = f.Article.Like ?? 0,
                    DisLike = f.Article.DisLike ?? 0,
                    CommentCount = f.Article.CommentCount ?? 0,
                    Picture = f.Article.Picture,
                    IsFavorited = true,
                    // 補上：在記憶體中重新計算人氣值，確保排序正確
                    Popular = (int)Math.Round(CalculatePopular(f.Article.Like, f.Article.DisLike, f.Article.CommentCount))
                }).ToList();

            //if (favoritesArticles == null)
            //{
            //    return Ok(new List<ArticleDTO>());
            //}

            return Ok(results);
        }

        private bool ArticleExists(int id) => _petSocietyContext.Articles.Any(e => e.ArticleId == id);
    }
}