using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface IShoppingCartService
    {
        Task<IEnumerable<ShoppingCartItemDto>> GetUserCartAsync(string userId);
        Task<ShoppingCartItemDto> AddToCartAsync(string userId, CreateShoppingCartItemDto createCartItemDto);
        Task<ShoppingCartItemDto> UpdateCartItemAsync(int cartItemId, UpdateShoppingCartItemDto updateCartItemDto);
        Task<bool> RemoveFromCartAsync(int cartItemId);
        Task<bool> ClearUserCartAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
        Task<bool> ValidateCartAsync(string userId);
    }
}