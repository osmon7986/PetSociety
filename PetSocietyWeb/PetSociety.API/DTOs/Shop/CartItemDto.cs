namespace PetSociety.API.DTOs.Shop
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? Thumbnail { get; set; } // 商品小圖
        public decimal Price { get; set; }    // 單價
        public int Quantity { get; set; }     // 購買數量

        // 小計：單價 * 數量 (這行可以寫在前端算，也可以後端算好給前端)
        public decimal SubTotal => Price * Quantity;
    }
}
