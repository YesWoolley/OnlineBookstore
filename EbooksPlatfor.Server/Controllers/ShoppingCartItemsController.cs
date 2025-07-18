using AutoMapper;
using EbooksPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ShoppingCartItemsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/shoppingcartitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingCartItemDto>>> GetShoppingCartItems()
        {
            try
            {
                var cartItems = await _context.ShoppingCartItems
                    .Include(sci => sci.User)
                    .Include(sci => sci.Book)
                    .ToListAsync();

                var cartItemDtos = _mapper.Map<IEnumerable<ShoppingCartItemDto>>(cartItems);
                return Ok(cartItemDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving shopping cart items", error = ex.Message });
            }
        }

        // GET: api/shoppingcartitems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingCartItemDto>> GetShoppingCartItem(int id)
        {
            try
            {
                var cartItem = await _context.ShoppingCartItems
                    .Include(sci => sci.User)
                    .Include(sci => sci.Book)
                    .FirstOrDefaultAsync(sci => sci.Id == id);

                if (cartItem == null)
                {
                    return NotFound(new { message = "Shopping cart item not found" });
                }

                var cartItemDto = _mapper.Map<ShoppingCartItemDto>(cartItem);
                return Ok(cartItemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the shopping cart item", error = ex.Message });
            }
        }

        // POST: api/shoppingcartitems
        [HttpPost]
        public async Task<IActionResult> CreateShoppingCartItem(CreateShoppingCartItemDto createCartItemDto)  // ← POST: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cartItem = _mapper.Map<ShoppingCartItem>(createCartItemDto);
                _context.ShoppingCartItems.Add(cartItem);
                await _context.SaveChangesAsync();

                // Reload the cart item with related data for the response
                await _context.Entry(cartItem)
                    .Reference(sci => sci.User)
                    .LoadAsync();
                await _context.Entry(cartItem)
                    .Reference(sci => sci.Book)
                    .LoadAsync();

                var cartItemDto = _mapper.Map<ShoppingCartItemDto>(cartItem);
                return CreatedAtAction(nameof(GetShoppingCartItem), new { id = cartItem.Id }, cartItemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the shopping cart item", error = ex.Message });
            }
        }

        // PUT: api/shoppingcartitems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShoppingCartItem(int id, UpdateShoppingCartItemDto updateCartItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cartItem = await _context.ShoppingCartItems.FindAsync(id);
                if (cartItem == null)
                {
                    return NotFound(new { message = "Shopping cart item not found" });
                }

                _mapper.Map(updateCartItemDto, cartItem);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the shopping cart item", error = ex.Message });
            }
        }

        // DELETE: api/shoppingcartitems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingCartItem(int id)
        {
            try
            {
                var cartItem = await _context.ShoppingCartItems.FindAsync(id);
                if (cartItem == null)
                {
                    return NotFound(new { message = "Shopping cart item not found" });
                }

                _context.ShoppingCartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the shopping cart item", error = ex.Message });
            }
        }
    }
}