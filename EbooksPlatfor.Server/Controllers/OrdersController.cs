using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;
using System.Security.Claims;

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
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                
                var orders = await _orderService.GetUserOrdersAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving orders", error = ex.Message });
            }
        }

        // GET: api/orders/user
        [HttpGet("user")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetCurrentUserOrders()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                
                var orders = await _orderService.GetUserOrdersAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user orders", error = ex.Message });
            }
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize]
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
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                
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