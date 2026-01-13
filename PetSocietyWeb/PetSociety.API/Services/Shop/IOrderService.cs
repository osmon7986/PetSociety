using PetSociety.API.DTOs.Shop;
using PetSociety.API.Models;

namespace PetSociety.API.Services.Shop
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderDto request);

        // 處理付款成功的回呼 (Callback) 
        Task HandlePaymentSuccessAsync(string merchantOrderNo, string tradeNo, string paymentDate);

        Task<List<OrderResponseDto>> GetOrdersByMemberIdAsync(int memberId);

        Task<Order> GetOrderByIdAsync(int orderId);

        Task CancelOrderAsync(int orderId, int memberId);

        Task DeleteOrderAsync(int orderId, int memberId);
    }
}
