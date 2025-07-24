using AutoMapper;
using EbooksPlatform.Models;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;

namespace OnlineBookstore.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ShoppingCartService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShoppingCartItemDto>> GetUserCartAsync(string userId)
        {
            var cartItems = await _context.ShoppingCartItems
                .Include(ci => ci.User)
                .Include(ci => ci.Book)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShoppingCartItemDto>>(cartItems);
        }

        public async Task<ShoppingCartItemDto> AddToCartAsync(string userId, CreateShoppingCartItemDto createCartItemDto)
        {
            // Validate book exists and has sufficient stock
            var book = await _context.Books.FindAsync(createCartItemDto.BookId);
            if (book == null)
                throw new ArgumentException("Book not found");

            if (book.StockQuantity < createCartItemDto.Quantity)
                throw new InvalidOperationException("Insufficient stock");

            // Check if item already exists in cart
            var existingItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.BookId == createCartItemDto.BookId);

            if (existingItem != null)
            {
                // Update quantity if item already exists
                existingItem.Quantity += createCartItemDto.Quantity;

                if (book.StockQuantity < existingItem.Quantity)
                    throw new InvalidOperationException("Insufficient stock for updated quantity");
            }
            else
            {
                // Create new cart item
                var cartItem = new ShoppingCartItem
                {
                    UserId = userId,
                    BookId = createCartItemDto.BookId,
                    Quantity = createCartItemDto.Quantity
                };
                _context.ShoppingCartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            // Reload with related data for response
            var updatedItem = await _context.ShoppingCartItems
                .Include(ci => ci.User)
                .Include(ci => ci.Book)
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.BookId == createCartItemDto.BookId);

            return _mapper.Map<ShoppingCartItemDto>(updatedItem);
        }

        public async Task<ShoppingCartItemDto> UpdateCartItemAsync(int cartItemId, UpdateShoppingCartItemDto updateCartItemDto)
        {
            var cartItem = await _context.ShoppingCartItems
                .Include(ci => ci.Book)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

            if (cartItem == null)
                throw new ArgumentException("Cart item not found");

            // Validate stock availability
            if (cartItem.Book.StockQuantity < updateCartItemDto.Quantity)
                throw new InvalidOperationException("Insufficient stock");

            cartItem.Quantity = updateCartItemDto.Quantity;
            await _context.SaveChangesAsync();

            // Reload with related data
            await _context.Entry(cartItem)
                .Reference(ci => ci.User)
                .LoadAsync();

            return _mapper.Map<ShoppingCartItemDto>(cartItem);
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId)
        {
            var cartItem = await _context.ShoppingCartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return false;

            _context.ShoppingCartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearUserCartAsync(string userId)
        {
            var cartItems = await _context.ShoppingCartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            _context.ShoppingCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var cartItems = await _context.ShoppingCartItems
                .Include(ci => ci.Book)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            return cartItems.Sum(ci => ci.Book.Price * ci.Quantity);
        }

        public async Task<bool> ValidateCartAsync(string userId)
        {
            var cartItems = await _context.ShoppingCartItems
                .Include(ci => ci.Book)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                if (item.Book.StockQuantity < item.Quantity)
                    return false;
            }

            return true;
        }
    }
}