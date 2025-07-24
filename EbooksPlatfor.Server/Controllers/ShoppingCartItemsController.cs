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