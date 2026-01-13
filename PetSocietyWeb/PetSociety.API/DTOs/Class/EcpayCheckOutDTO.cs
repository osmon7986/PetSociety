using System.ComponentModel.DataAnnotations;

namespace PetSociety.API.DTOs.Class
{
    public class EcpayCheckOutDTO
    {
        [Required(ErrorMessage = "訂單編號必填")]
        public string OrderId { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "金額必須大於零")]
        public int Amount { get; set; }
        [Required(ErrorMessage = "商品不可無")]
        public string ItemName { get; set; } = string.Empty;
        public string? ClientBackUrl { get; set; }
    }
}
