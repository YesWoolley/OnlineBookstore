using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;

namespace OnlineBookstore.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IShoppingCartService _cartService;

        public OrderService(AppDbContext context, IMapper mapper, IShoppingCartService cartService)
        {
            _context = context;
            _mapper = mapper;
            _cartService = cartService;
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Book)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            return order != null ? _mapper.Map<OrderDto>(order) : null;
        }

        public async Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto createOrderDto)
        {
            // Validate cart before creating order
            if (!await _cartService.ValidateCartAsync(userId))
                throw new InvalidOperationException("Cart validation failed - insufficient stock");

            var cartItems = await _context.ShoppingCartItems
                .Include(ci => ci.Book)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new InvalidOperationException("Cart is empty");

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = createOrderDto.ShippingAddress,
                OrderStatus = "Pending",
                TotalAmount = 0 // Will be calculated
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create order items and calculate total
            decimal totalAmount = 0;
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Book.Price
                };

                _context.OrderItems.Add(orderItem);
                totalAmount += cartItem.Book.Price * cartItem.Quantity;

                // Update book stock
                cartItem.Book.StockQuantity -= cartItem.Quantity;
            }

            // Update order total
            order.TotalAmount = totalAmount;
            await _context.SaveChangesAsync();

            // Clear user's cart
            await _cartService.ClearUserCartAsync(userId);

            // Reload with related data for response
            var createdOrder = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return _mapper.Map<OrderDto>(createdOrder);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found");

            // Validate status transition
            var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(newStatus))
                throw new ArgumentException("Invalid order status");

            order.OrderStatus = newStatus;
            await _context.SaveChangesAsync();

            // Reload with related data
            var updatedOrder = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            return _mapper.Map<OrderDto>(updatedOrder);
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            if (order.OrderStatus == "Delivered")
                throw new InvalidOperationException("Cannot cancel delivered order");

            // Restore book stock
            foreach (var orderItem in order.OrderItems!)
            {
                orderItem.Book.StockQuantity += orderItem.Quantity;
            }

            order.OrderStatus = "Cancelled";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<OrderDto> ConvertCartToOrderAsync(string userId, string shippingAddress)
        {
            var createOrderDto = new CreateOrderDto
            {
                ShippingAddress = shippingAddress,
                OrderStatus = "Pending",
                OrderItems = new List<CreateOrderItemDto>() // Will be populated from cart
            };

            return await CreateOrderAsync(userId, createOrderDto);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Book)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Book)
                .Where(o => o.OrderStatus == status)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }
    }
}