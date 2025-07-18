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
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ReviewsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var reviews = await _context.Reviews  // ← Get reviews from database
                    .Include(r => r.Book)            // ← Load related book data
                    .Include(r => r.User)            // ← Load related user data
                    .ToListAsync();                  // ← Execute query and return list

                var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
                return Ok(reviewDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving reviews", error = ex.Message });
            }
        }

        // GET: api/reviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var review = await _context.Reviews  // ← Get review from database
                    .Include(r => r.Book)           // ← Load related book data
                    .Include(r => r.User)           // ← Load related user data
                    .FirstOrDefaultAsync(r => r.Id == id);  // ← Execute query and return single review

                if (review == null)
                {
                    return NotFound(new { message = "Review not found" });
                }

                var reviewDto = _mapper.Map<ReviewDto>(review);
                return Ok(reviewDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the review", error = ex.Message });
            }
        }

        // POST: api/reviews
        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewDto createReviewDto)  // ← POST: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var review = _mapper.Map<Review>(createReviewDto);
                review.CreatedAt = DateTime.UtcNow;
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                // Reload the review with related data for the response
                await _context.Entry(review)
                    .Reference(r => r.Book)
                    .LoadAsync();
                await _context.Entry(review)
                    .Reference(r => r.User)
                    .LoadAsync();

                var reviewDto = _mapper.Map<ReviewDto>(review);
                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, reviewDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the review", error = ex.Message });
            }
        }

        // PUT: api/reviews/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, UpdateReviewDto updateReviewDto)  // ← PUT: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var review = await _context.Reviews.FindAsync(id);
                if (review == null)
                {
                    return NotFound(new { message = "Review not found" });
                }

                _mapper.Map(updateReviewDto, review);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the review", error = ex.Message });
            }
        }

        // DELETE: api/reviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)  // ← DELETE: Use IActionResult (different success responses)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);
                if (review == null)
                {
                    return NotFound(new { message = "Review not found" });
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the review", error = ex.Message });
            }
        }
    }
}