using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PetSocietyWeb.ViewModels
{
    public class ProductEditViewModel
    {
        // 編輯時必須知道是改哪一筆，所以要有 Id
        public int ProductId { get; set; }

        [Display(Name = "商品名稱")]
        [Required(ErrorMessage = "請輸入{0}")]
        public string ProductName { get; set; }

        [Display(Name = "商品描述")]
        [Required(ErrorMessage = "請輸入{0}")]
        public string Description { get; set; }

        [Display(Name = "單價")]
        [Required(ErrorMessage = "請輸入{0}")]
        [Range(0, int.MaxValue, ErrorMessage = "{0}不能小於 0")]
        public decimal Price { get; set; }

        [Display(Name = "庫存數量")]
        [Required(ErrorMessage = "請輸入{0}")]
        [Range(0, int.MaxValue, ErrorMessage = "{0}不能小於 0")]
        public int Stock { get; set; }

        [Display(Name = "商品類別")]
        [Required(ErrorMessage = "請選擇{0}")]
        public int CategoryId { get; set; }

        [Display(Name = "賣家信箱")]
        [Required(ErrorMessage = "請選擇{0}")]
        public int MemberId { get; set; }

        [Display(Name = "商品狀態")]
        public string Status { get; set; }

        public List<string> ImageUrlList { get; set; } = new List<string>();

        // (原本的 Photos 欄位保持不變)
        [Display(Name = "更新圖片 (若不修改請留空)")]
        public List<IFormFile>? Photos { get; set; }
    }
}