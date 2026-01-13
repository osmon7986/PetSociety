namespace PetSociety.API.DTOs.Shop
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        // 新增：付款成功日期 (可能是 null，所以用 DateTime?)
        public DateTime? PaymentDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ReceiverName { get; set; }  
        public string ReceiverAddress { get; set; }
        public string ReceiverPhone { get; set; }
        // ★ 重點：這裡放的是我們自定義的「明細 DTO」，不是原本的 OrderDetail
        public List<OrderDetailResponseDto> Details { get; set; } = new List<OrderDetailResponseDto>();
    }
}
