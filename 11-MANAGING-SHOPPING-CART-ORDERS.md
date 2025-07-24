# Section 11: Managing Shopping Cart & Orders

Welcome to the shopping cart and order management phase! In this section, we'll create the backend services for shopping cart operations and order processing. This involves complex business logic for inventory management, price calculations, and order status tracking.

---

## 🎯 What You'll Learn

- How to create backend services for shopping cart operations
- How to implement order processing with inventory management
- How to handle complex business logic for pricing and stock validation
- How to manage order status transitions
- How to implement cart-to-order conversion
- How to structure services for e-commerce functionality

---

## 🏗️ Step 1: Create Shopping Cart DTOs

### **Create DTOs/ShoppingCartItemDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends cart item data TO clients with book details
    public class ShoppingCartItemDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public decimal BookPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // Input DTO: Receives cart item data FROM clients for creation
    public class CreateShoppingCartItemDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    // Input DTO: Receives cart item data FROM clients for updates
    public class UpdateShoppingCartItemDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
```

---

## 🏗️ Step 2: Create Order DTOs

### **Create DTOs/OrderDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends order data TO clients with user and items
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    // Input DTO: Receives order data FROM clients for creation
    public class CreateOrderDto
    {
        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string OrderStatus { get; set; } = null!;

        [Required]
        public ICollection<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }

    // Input DTO: Receives order data FROM clients for updates
    public class UpdateOrderDto
    {
        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string OrderStatus { get; set; } = null!;
    }
}
```

### **Create DTOs/OrderItemDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends order item data TO clients
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // Input DTO: Receives order item data FROM clients
    public class CreateOrderItemDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    // Input DTO: Receives order item data FROM clients for updates
    public class UpdateOrderItemDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
```

---

## 🎮 Step 3: Create Shopping Cart Service

### **Create Services/IShoppingCartService.cs:**
```csharp
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
```

### **Create Services/ShoppingCartService.cs:**
```csharp
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;

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
```

---

## 🎮 Step 4: Create Order Service

### **Create Services/IOrderService.cs:**
```csharp
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
```

### **Create Services/OrderService.cs:**
```csharp
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
```

---

## 🎮 Step 5: Create OrderItem Service

### **Create Services/IOrderItemService.cs:**
```csharp
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
```

### **Create Services/OrderItemService.cs:**
```csharp
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
```

---

## 🎮 Step 6: Create Controllers

### **Create Controllers/ShoppingCartController.cs:**
```csharp
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _cartService;

        public ShoppingCartController(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/shoppingcart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingCartItemDto>>> GetUserCart()
        {
            try
            {
                // TODO: Get userId from authenticated user
                var userId = "current-user-id"; // Replace with actual user ID
                var cartItems = await _cartService.GetUserCartAsync(userId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving cart", error = ex.Message });
            }
        }

        // POST: api/shoppingcart
        [HttpPost]
        public async Task<IActionResult> AddToCart(CreateShoppingCartItemDto createCartItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = "current-user-id"; // Replace with actual user ID
                var cartItem = await _cartService.AddToCartAsync(userId, createCartItemDto);
                return CreatedAtAction(nameof(GetUserCart), cartItem);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding to cart", error = ex.Message });
            }
        }

        // PUT: api/shoppingcart/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, UpdateShoppingCartItemDto updateCartItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _cartService.UpdateCartItemAsync(id, updateCartItemDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating cart item", error = ex.Message });
            }
        }

        // DELETE: api/shoppingcart/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            try
            {
                var result = await _cartService.RemoveFromCartAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = "Cart item not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing from cart", error = ex.Message });
            }
        }

        // DELETE: api/shoppingcart/clear
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = "current-user-id"; // Replace with actual user ID
                await _cartService.ClearUserCartAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while clearing cart", error = ex.Message });
            }
        }

        // GET: api/shoppingcart/total
        [HttpGet("total")]
        public async Task<ActionResult<decimal>> GetCartTotal()
        {
            try
            {
                var userId = "current-user-id"; // Replace with actual user ID
                var total = await _cartService.GetCartTotalAsync(userId);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while calculating total", error = ex.Message });
            }
        }
    }
}
```

### **Create Controllers/OrdersController.cs:**
```csharp
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders()
        {
            try
            {
                var userId = "current-user-id"; // Replace with actual user ID
                var orders = await _orderService.GetUserOrdersAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving orders", error = ex.Message });
            }
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                
                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the order", error = ex.Message });
            }
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = "current-user-id"; // Replace with actual user ID
                var order = await _orderService.CreateOrderAsync(userId, createOrderDto);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order", error = ex.Message });
            }
        }

        // PUT: api/orders/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string newStatus)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, newStatus);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating order status", error = ex.Message });
            }
        }

        // DELETE: api/orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = "Order not found" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while cancelling the order", error = ex.Message });
            }
        }

        // POST: api/orders/convert-cart
        [HttpPost("convert-cart")]
        public async Task<IActionResult> ConvertCartToOrder([FromBody] string shippingAddress)
        {
            try
            {
                var userId = "current-user-id"; // Replace with actual user ID
                var order = await _orderService.ConvertCartToOrderAsync(userId, shippingAddress);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while converting cart to order", error = ex.Message });
            }
        }
    }
}
```

### **Create Controllers/OrderItemsController.cs:**
```csharp
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/orders/{orderId}/[controller]")]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemsController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        // GET: api/orders/5/orderitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItems(int orderId)
        {
            try
            {
                var orderItems = await _orderItemService.GetOrderItemsAsync(orderId);
                return Ok(orderItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving order items", error = ex.Message });
            }
        }

        // GET: api/orders/5/orderitems/2
        [HttpGet("{itemId}")]
        public async Task<ActionResult<OrderItemDto>> GetOrderItem(int orderId, int itemId)
        {
            try
            {
                var orderItem = await _orderItemService.GetOrderItemByIdAsync(orderId, itemId);
                
                if (orderItem == null)
                {
                    return NotFound(new { message = "Order item not found" });
                }

                return Ok(orderItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the order item", error = ex.Message });
            }
        }

        // POST: api/orders/5/orderitems
        [HttpPost]
        public async Task<IActionResult> CreateOrderItem(int orderId, CreateOrderItemDto createOrderItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var orderItem = await _orderItemService.CreateOrderItemAsync(orderId, createOrderItemDto);
                return CreatedAtAction(nameof(GetOrderItem), new { orderId, itemId = orderItem.Id }, orderItem);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order item", error = ex.Message });
            }
        }

        // PUT: api/orders/5/orderitems/2
        [HttpPut("{itemId}")]
        public async Task<IActionResult> UpdateOrderItem(int orderId, int itemId, UpdateOrderItemDto updateOrderItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _orderItemService.UpdateOrderItemAsync(orderId, itemId, updateOrderItemDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the order item", error = ex.Message });
            }
        }

        // DELETE: api/orders/5/orderitems/2
        [HttpDelete("{itemId}")]
        public async Task<IActionResult> DeleteOrderItem(int orderId, int itemId)
        {
            try
            {
                var result = await _orderItemService.DeleteOrderItemAsync(orderId, itemId);
                
                if (!result)
                {
                    return NotFound(new { message = "Order item not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the order item", error = ex.Message });
            }
        }

        // GET: api/orders/5/orderitems/total
        [HttpGet("total")]
        public async Task<ActionResult<decimal>> GetOrderTotal(int orderId)
        {
            try
            {
                var total = await _orderItemService.GetOrderTotalAsync(orderId);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while calculating order total", error = ex.Message });
            }
        }
    }
}
```

---

## ⚙️ Step 7: Update AutoMapper Profile

### **Update Mapppings/AutoMapperProfile.cs:**
```csharp
using AutoMapper;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;

namespace OnlineBookstore.Mapppings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Existing mappings...

            // Shopping Cart mappings
            CreateMap<ShoppingCartItem, ShoppingCartItemDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.BookPrice, opt => opt.MapFrom(src => src.Book.Price))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Book.Price * src.Quantity));

            CreateMap<CreateShoppingCartItemDto, ShoppingCartItem>();
            CreateMap<UpdateShoppingCartItemDto, ShoppingCartItem>();

            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.UnitPrice * src.Quantity));

            CreateMap<CreateOrderDto, Order>();
            CreateMap<UpdateOrderDto, Order>();
            CreateMap<CreateOrderItemDto, OrderItem>();
        }
    }
}
```

---

## ⚙️ Step 7: Register Services in Program.cs

### **Update Program.cs:**
```csharp
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using OnlineBookstore.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Register Services
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>(); // ----add
builder.Services.AddScoped<IOrderService, OrderService>(); // ----add
builder.Services.AddScoped<IOrderItemService, OrderItemService>(); // ----add
 
// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Seed the database
await DbInitializer.SeedAsync(app);

app.Run();
```

---

## 🧪 Step 8: Test Your Shopping Cart & Order Management

1. **Start the backend application:**
   ```bash
   # Backend
   dotnet run
   ```

2. **Test API endpoints using Postman or curl:**
   ```bash
# Shopping Cart Operations
GET https://localhost:7273/api/shoppingcart
POST https://localhost:7273/api/shoppingcart
Content-Type: application/json
{
  "bookId": 1,
  "quantity": 2
}

PUT https://localhost:7273/api/shoppingcart/1
Content-Type: application/json
{
  "quantity": 3
}

DELETE https://localhost:7273/api/shoppingcart/1
DELETE https://localhost:7273/api/shoppingcart/clear
GET https://localhost:7273/api/shoppingcart/total

# Order Operations
GET https://localhost:7273/api/orders
GET https://localhost:7273/api/orders/1

POST https://localhost:7273/api/orders
Content-Type: application/json
{
  "shippingAddress": "123 Main St, City, State 12345",
  "orderStatus": "Pending",
  "orderItems": [
    {
      "bookId": 1,
      "quantity": 2
    }
  ]
}

PUT https://localhost:7273/api/orders/1/status
Content-Type: application/json
"Shipped"

DELETE https://localhost:7273/api/orders/1

POST https://localhost:7273/api/orders/convert-cart
Content-Type: application/json
"123 Main St, City, State 12345"

# Order Item Operations
GET https://localhost:7273/api/orders/1/orderitems
GET https://localhost:7273/api/orders/1/orderitems/2

POST https://localhost:7273/api/orders/1/orderitems
Content-Type: application/json
{
  "bookId": 1,
  "quantity": 2
}

PUT https://localhost:7273/api/orders/1/orderitems/2
Content-Type: application/json
{
  "quantity": 3
}

DELETE https://localhost:7273/api/orders/1/orderitems/2
GET https://localhost:7273/api/orders/1/orderitems/total
   ```

3. **Test the Swagger UI:**
   - Navigate to `https://localhost:7273/swagger`
   - Test all the shopping cart and order endpoints directly from the browser

---

## 🏆 Best Practices

### **Shopping Cart Best Practices:**
- **Stock validation** before adding items
- **Quantity limits** to prevent abuse
- **Cart expiration** for inactive users
- **Price calculations** with proper decimal handling
- **User-specific carts** with proper isolation

### **Order Management Best Practices:**
- **Inventory management** with stock updates
- **Order status tracking** with valid transitions
- **Price calculations** with tax and shipping
- **Order cancellation** with stock restoration
- **Audit trail** for order changes

### **E-commerce Best Practices:**
- **Transaction consistency** across cart and orders
- **Stock reservation** during checkout
- **Payment integration** (to be added later)
- **Order confirmation** emails
- **Return/refund** processing

---

## ✅ What You've Accomplished

- ✅ Created **shopping cart service** with complete CRUD operations
- ✅ **Implemented inventory management** with stock validation
- ✅ **Added order processing** with status tracking
- ✅ **Created cart-to-order conversion** functionality
- ✅ **Implemented price calculations** and totals
- ✅ **Added order cancellation** with stock restoration
- ✅ **Set up proper validation** for quantities and addresses
- ✅ **Created comprehensive API endpoints** for cart and order management

---

## 🚀 Next Steps

Your shopping cart and order management backend is now complete! In the next section, we'll implement payment integration.

**You've successfully created a complete e-commerce backend with cart and order management. Great job!**

---

**Next up:**
- [Section 14: Integrating Payments](./14-INTEGRATING-PAYMENTS.md)