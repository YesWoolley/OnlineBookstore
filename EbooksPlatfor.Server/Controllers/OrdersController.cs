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
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrdersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems!)
                        .ThenInclude(oi => oi.Book)
                    .ToListAsync();

                var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving orders", error = ex.Message });
            }
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems!)
                        .ThenInclude(oi => oi.Book)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                var orderDto = _mapper.Map<OrderDto>(order);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the order", error = ex.Message });
            }
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)  // ← POST: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = _mapper.Map<Order>(createOrderDto);
                order.OrderDate = DateTime.UtcNow;
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Reload the order with related data for the response
                await _context.Entry(order)
                    .Reference(o => o.User)
                    .LoadAsync();
                await _context.Entry(order)
                    .Collection(o => o.OrderItems!)
                    .Query()
                    .Include(oi => oi.Book)
                    .LoadAsync();

                var orderDto = _mapper.Map<OrderDto>(order);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order", error = ex.Message });
            }
        }

        // PUT: api/orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto updateOrderDto)  // ← PUT: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                _mapper.Map(updateOrderDto, order);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the order", error = ex.Message });
            }
        }

        // DELETE: api/orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)  // ← DELETE: Use IActionResult (different success responses)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the order", error = ex.Message });
            }
        }
    }
}