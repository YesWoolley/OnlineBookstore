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