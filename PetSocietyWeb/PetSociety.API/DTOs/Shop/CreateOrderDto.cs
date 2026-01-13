namespace PetSociety.API.DTOs.Shop
{
    public class CreateOrderDto
    {
        public int MemberId { get; set; }

        public int? UserCouponId { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } // "Credit", "ATM", "CVS"

        public ReceiverDto Receiver { get; set; }

        public List<OrderItemDto> OrderItems { get; set; }
    }

    public class ReceiverDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
