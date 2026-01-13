namespace PetSociety.API.DTOs.User
{
        // 用於 "新增" 收藏 (前端傳給後端的)
        public class AddFavoriteDto
        {
            public int TargetId { get; set; }
            public int TargetType { get; set; }
            public string Title { get; set; }
            public string? ImageUrl { get; set; }
            public decimal? Price { get; set; }
            public string? Intro { get; set; }
        }

        // 用於 "顯示" 收藏列表 (後端傳給前端的)
        public class FavoriteItemDto
        {
            public int FavoriteId { get; set; }
            public int TargetId { get; set; }
            public int TargetType { get; set; }
            public string Title { get; set; }
            public string? ImageUrl { get; set; }
            public decimal? Price { get; set; }
            public string? Intro { get; set; }
            public string CreateDate { get; set; } // 格式化後的日期字串
        }
}
