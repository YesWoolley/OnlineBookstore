using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto createOrderDto);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<bool> CancelOrderAsync(int orderId);
        Task<OrderDto> ConvertCartToOrderAsync(string userId, string shippingAddress);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status);
    }
}