using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderItemsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/orderitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItems()  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var orderItems = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .Include(oi => oi.Book)
                    .ToListAsync();

                var orderItemDtos = _mapper.Map<IEnumerable<OrderItemDto>>(orderItems);
                return Ok(orderItemDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving order items", error = ex.Message });
            }
        }

        // GET: api/orderitems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemDto>> GetOrderItem(int id)  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var orderItem = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .Include(oi => oi.Book)
                    .FirstOrDefaultAsync(oi => oi.Id == id);

                if (orderItem == null)
                {
                    return NotFound(new { message = "Order item not found" });
                }

                var orderItemDto = _mapper.Map<OrderItemDto>(orderItem);
                return Ok(orderItemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the order item", error = ex.Message });
            }
        }

        // POST: api/orderitems
        [HttpPost]
        public async Task<IActionResult> CreateOrderItem(CreateOrderItemDto createOrderItemDto)  // ← POST: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var orderItem = _mapper.Map<OrderItem>(createOrderItemDto);
                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();

                // Reload the order item with related data for the response
                await _context.Entry(orderItem)
                    .Reference(oi => oi.Book)
                    .LoadAsync();

                var orderItemDto = _mapper.Map<OrderItemDto>(orderItem);
                return CreatedAtAction(nameof(GetOrderItem), new { id = orderItem.Id }, orderItemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order item", error = ex.Message });
            }
        }

        // PUT: api/orderitems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(int id, UpdateOrderItemDto updateOrderItemDto)  // ← PUT: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var orderItem = await _context.OrderItems.FindAsync(id);
                if (orderItem == null)
                {
                    return NotFound(new { message = "Order item not found" });
                }

                _mapper.Map(updateOrderItemDto, orderItem);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the order item", error = ex.Message });
            }
        }

        // DELETE: api/orderitems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)  // ← DELETE: Use IActionResult (different success responses)
        {
            try
            {
                var orderItem = await _context.OrderItems.FindAsync(id);
                if (orderItem == null)
                {
                    return NotFound(new { message = "Order item not found" });
                }

                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the order item", error = ex.Message });
            }
        }
    }
}