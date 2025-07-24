using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET: api/reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()
        {
            try
            {
                var reviews = await _reviewService.GetAllReviewsAsync();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving reviews", error = ex.Message });
            }
        }

        // GET: api/reviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id);

                if (review == null)
                {
                    return NotFound(new { message = "Review not found" });
                }

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the review", error = ex.Message });
            }
        }

        // GET: api/reviews/book/5
        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByBook(int bookId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByBookAsync(bookId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving book reviews", error = ex.Message });
            }
        }

        // GET: api/reviews/user/current
        [HttpGet("user/current")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetUserReviews()
        {
            try
            {
                var userId = "current-user-id"; // Replace with actual user ID
                var reviews = await _reviewService.GetReviewsByUserAsync(userId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user reviews", error = ex.Message });
            }
        }

        // GET: api/reviews/book/5/average-rating
        [HttpGet("book/{bookId}/average-rating")]
        public async Task<ActionResult<double>> GetAverageRating(int bookId)
        {
            try
            {
                var averageRating = await _reviewService.GetAverageRatingAsync(bookId);
                return Ok(averageRating);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while calculating average rating", error = ex.Message });
            }
        }

        // GET: api/reviews/book/5/review-count
        [HttpGet("book/{bookId}/review-count")]
        public async Task<ActionResult<int>> GetReviewCount(int bookId)
        {
            try
            {
                var reviewCount = await _reviewService.GetReviewCountAsync(bookId);
                return Ok(reviewCount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while getting review count", error = ex.Message });
            }
        }

        // GET: api/reviews/book/5/has-reviewed
        [HttpGet("book/{bookId}/has-reviewed")]
        public async Task<ActionResult<bool>> HasUserReviewedBook(int bookId)
        {
            try
            {
                var userId = "current-user-id"; // Replace with actual user ID
                var hasReviewed = await _reviewService.HasUserReviewedBookAsync(userId, bookId);
                return Ok(hasReviewed);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking review status", error = ex.Message });
            }
        }

        // POST: api/reviews
        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewDto createReviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = "current-user-id"; // Replace with actual user ID
                var review = await _reviewService.CreateReviewAsync(userId, createReviewDto);
                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
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
                return StatusCode(500, new { message = "An error occurred while creating the review", error = ex.Message });
            }
        }

        // PUT: api/reviews/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, UpdateReviewDto updateReviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = "current-user-id"; // Replace with actual user ID
                await _reviewService.UpdateReviewAsync(id, userId, updateReviewDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the review", error = ex.Message });
            }
        }

        // DELETE: api/reviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                var userId = "current-user-id"; // Replace with actual user ID
                var result = await _reviewService.DeleteReviewAsync(id, userId);

                if (!result)
                {
                    return NotFound(new { message = "Review not found" });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the review", error = ex.Message });
            }
        }
    }
}