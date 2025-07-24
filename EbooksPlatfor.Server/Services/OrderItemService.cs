using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;

namespace OnlineBookstore.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderItemService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(int orderId)
        {
            var orderItems = await _context.OrderItems
                .Include(oi => oi.Book)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderItemDto>>(orderItems);
        }

        public async Task<OrderItemDto?> GetOrderItemByIdAsync(int orderId, int itemId)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Book)
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.Id == itemId);

            return orderItem != null ? _mapper.Map<OrderItemDto>(orderItem) : null;
        }

        public async Task<OrderItemDto> CreateOrderItemAsync(int orderId, CreateOrderItemDto createOrderItemDto)
        {
            // Validate order exists
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found");

            // Validate book exists and has sufficient stock
            var book = await _context.Books.FindAsync(createOrderItemDto.BookId);
            if (book == null)
                throw new ArgumentException("Book not found");

            if (book.StockQuantity < createOrderItemDto.Quantity)
                throw new InvalidOperationException("Insufficient stock");

            // Check if item already exists in order
            var existingItem = await _context.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.BookId == createOrderItemDto.BookId);

            if (existingItem != null)
            {
                // Update quantity if item already exists
                existingItem.Quantity += createOrderItemDto.Quantity;

                if (book.StockQuantity < existingItem.Quantity)
                    throw new InvalidOperationException("Insufficient stock for updated quantity");
            }
            else
            {
                // Create new order item
                var orderItem = new OrderItem
                {
                    OrderId = orderId,
                    BookId = createOrderItemDto.BookId,
                    Quantity = createOrderItemDto.Quantity,
                    UnitPrice = book.Price
                };
                _context.OrderItems.Add(orderItem);
            }

            // Update book stock
            book.StockQuantity -= createOrderItemDto.Quantity;

            await _context.SaveChangesAsync();

            // Reload with related data for response
            var createdItem = await _context.OrderItems
                .Include(oi => oi.Book)
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.BookId == createOrderItemDto.BookId);

            return _mapper.Map<OrderItemDto>(createdItem);
        }

        public async Task<OrderItemDto> UpdateOrderItemAsync(int orderId, int itemId, UpdateOrderItemDto updateOrderItemDto)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Book)
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.Id == itemId);

            if (orderItem == null)
                throw new ArgumentException("Order item not found");

            // Validate stock availability
            if (orderItem.Book.StockQuantity < updateOrderItemDto.Quantity)
                throw new InvalidOperationException("Insufficient stock");

            // Update quantity
            orderItem.Quantity = updateOrderItemDto.Quantity;
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderItemDto>(orderItem);
        }

        public async Task<bool> DeleteOrderItemAsync(int orderId, int itemId)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Book)
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.Id == itemId);

            if (orderItem == null)
                return false;

            // Restore book stock
            orderItem.Book.StockQuantity += orderItem.Quantity;

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> GetOrderTotalAsync(int orderId)
        {
            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

            return orderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
        }
    }
}