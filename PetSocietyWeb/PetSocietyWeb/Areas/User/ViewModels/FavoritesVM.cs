namespace PetSocietyWeb.Areas.User.ViewModels
{
    public class FavoriteVM
    {
        public int FavoriteId { get; set; }
        public int MemberId { get; set; }
        public int TargetId { get; set; }
        public int TargetType { get; set; }
        public DateTime CreateDate { get; set; }


        //額外給畫面用的欄位：顯示收藏的東西
        public string TargetName { get; set; }    // 商品/課程/文章名稱
        public string ImageUrl { get; set; }      // 圖片
        public string TypeName { get; set; }      // 類別名稱(課程/文章/商品)
    }
}
