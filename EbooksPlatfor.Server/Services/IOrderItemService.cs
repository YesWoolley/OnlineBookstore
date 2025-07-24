using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(int orderId);
        Task<OrderItemDto?> GetOrderItemByIdAsync(int orderId, int itemId);
        Task<OrderItemDto> CreateOrderItemAsync(int orderId, CreateOrderItemDto createOrderItemDto);
        Task<OrderItemDto> UpdateOrderItemAsync(int orderId, int itemId, UpdateOrderItemDto updateOrderItemDto);
        Task<bool> DeleteOrderItemAsync(int orderId, int itemId);
        Task<decimal> GetOrderTotalAsync(int orderId);
    }
}