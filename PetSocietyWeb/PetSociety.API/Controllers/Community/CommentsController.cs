using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Community;
using PetSociety.API.Models;

namespace PetSociety.API.Controllers.Community
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly PetSocietyContext _context;
        public CommentsController(PetSocietyContext context) => _context = context;

        [HttpGet("Article/{articleId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetCommentsByArticle(int articleId)
        {
            var comments = await _context.Comments
                .Where(c => c.ArticleId == articleId)
                .OrderBy(c => c.PostDate) // 按時間排序
                .Select(c => new {
                    c.CommentId,
                    c.ArticleId,
                    c.MemberId,
                    c.Content,
                    c.PostDate,
                    // 關鍵：直接在 SQL 查詢時抓出會員名字
                    MemberName = c.Member.Name
                })
                .ToListAsync();

            return Ok(comments);
        }

        // POST: api/Comments
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(CommentDTO dto)
        {
            try
            {
                var comment = new Comment
                {
                    ArticleId = dto.ArticleId,
                    MemberId = dto.MemberId, // 實務上應從 User.Identity 取得
                    Content = dto.Content,
                    PostDate = DateTime.Now
                };

                _context.Comments.Add(comment);

                // 同步更新文章的回覆次數 (CommentCount)
                var article = await _context.Articles.FindAsync(dto.ArticleId);
                if (article != null)
                {
                    article.CommentCount = (article.CommentCount ?? 0) + 1;
                }
                await _context.SaveChangesAsync();

                // 抓取留言者名稱回傳給前端
                var memberName = await _context.Members
                    .Where(m => m.MemberId == dto.MemberId)
                    .Select(m => m.Name)
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    commentId = comment.CommentId,
                    articleId = comment.ArticleId,
                    memberId = comment.MemberId,
                    content = comment.Content,
                    postDate = comment.PostDate
                });
            }
            catch (Exception ex)
            {
                return BadRequest("留言失敗，文章關聯資料有誤：" + ex.Message);
            }
        }
    }
}
