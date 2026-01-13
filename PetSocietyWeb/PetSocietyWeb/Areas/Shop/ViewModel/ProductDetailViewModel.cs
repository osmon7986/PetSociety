using System;

namespace PetSocietyWeb.ViewModels
{
    public class ProductDetailViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CategoryName { get; set; }
        public string SellerEmail { get; set; }
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }

        // ★ 修改：改用 List 存多張圖
        public List<string> ImageUrlList { get; set; } = new List<string>();
    }
}