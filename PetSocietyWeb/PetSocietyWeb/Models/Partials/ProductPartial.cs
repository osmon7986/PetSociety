using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace PetSocietyWeb.Models.Generated
{
    [ModelMetadataType(typeof(ProductMetadata))]
    public partial class Product
    {

    }

    public class ProductMetadata
    {
        [Display(Name = "商品名稱")]
        [Required(ErrorMessage = "請輸入{0}")]
        public string ProductName { get; set; }

        [Display(Name = "商品類別")]
        [Required(ErrorMessage = "請選擇{0}")]
        public int? CategoryId { get; set; }

        [Display(Name = "賣家信箱")]
        [Required(ErrorMessage = "請選擇{0}")]
        public int? MemberId { get; set; }

        [Display(Name = "單價")]
        [Required(ErrorMessage = "請輸入{0}")]
        // Range: 設定範圍 0 到 最大值，解決負數 bug
        [Range(0, double.MaxValue, ErrorMessage = "{0}不能小於 0")]
        public decimal? Price { get; set; }

        [Display(Name = "數量")]
        [Required(ErrorMessage = "請輸入{0}")]
        // Range: 設定整數範圍 0 到 最大值
        [Range(0, int.MaxValue, ErrorMessage = "{0}不能小於 0")]
        public int? Stock { get; set; }

        [Display(Name = "商品描述")]
        [Required(ErrorMessage = "請輸入{0}")]
        public string Description { get; set; }

        [Display(Name = "商品狀態")]
        public string Status { get; set; }

        [Display(Name = "建立時間")]
        public DateTime? CreateDate { get; set; }
    }
}