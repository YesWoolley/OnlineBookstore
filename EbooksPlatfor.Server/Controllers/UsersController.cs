using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;
using System.Security.Claims;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IShoppingCartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IReviewService _reviewService;

        public UsersController(
            IShoppingCartService cartService,
            IOrderService orderService,
            IReviewService reviewService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _reviewService = reviewService;
        }

        // GET: api/users/cart
        [HttpGet("cart")]
        public async Task<ActionResult<IEnumerable<ShoppingCartItemDto>>> GetUserCart()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var cartItems = await _cartService.GetUserCartAsync(userId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving cart", error = ex.Message });
            }
        }

        // GET: api/users/orders
        [HttpGet("orders")]
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

        // GET: api/users/reviews
        [HttpGet("reviews")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetUserReviews()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var reviews = await _reviewService.GetReviewsByUserAsync(userId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving reviews", error = ex.Message });
            }
        }
    }
}