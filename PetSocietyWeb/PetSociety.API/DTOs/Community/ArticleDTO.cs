using PetSociety.API.Models;

namespace PetSociety.API.DTOs.Community
{
    // 文章DTO
    public class ArticleDTO
    {
        public int ArticleId { get; set; }

        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int TagId { get; set; }

        public string? TagName { get; set; }

        public int MemberId { get; set; }

        public string? MemberName { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime PostDate { get; set; }

        public DateTime LastUpdate { get; set; }

        public int? Like { get; set; }

        public int? DisLike { get; set; }

        public int? Popular { get; set; }

        public int? CommentCount { get; set; }

        public string? Picture { get; set; }

        public bool? IsFavorited { get; set; }
    }
}
