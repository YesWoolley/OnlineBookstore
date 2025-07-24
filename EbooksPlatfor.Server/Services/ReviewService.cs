using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;

namespace OnlineBookstore.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ReviewService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByBookAsync(int bookId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(string userId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            return review != null ? _mapper.Map<ReviewDto>(review) : null;
        }

        public async Task<ReviewDto> CreateReviewAsync(string userId, CreateReviewDto createReviewDto)
        {
            // Validate book exists
            var book = await _context.Books.FindAsync(createReviewDto.BookId);
            if (book == null)
                throw new ArgumentException("Book not found");

            // Check if user has already reviewed this book
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == createReviewDto.BookId);

            if (existingReview != null)
                throw new InvalidOperationException("User has already reviewed this book");

            // Validate rating
            if (createReviewDto.Rating < 1 || createReviewDto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var review = new Review
            {
                BookId = createReviewDto.BookId,
                UserId = userId,
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Reload with related data for response
            await _context.Entry(review)
                .Reference(r => r.Book)
                .LoadAsync();
            await _context.Entry(review)
                .Reference(r => r.User)
                .LoadAsync();

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewDto> UpdateReviewAsync(int id, string userId, UpdateReviewDto updateReviewDto)
        {
            var review = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                throw new ArgumentException("Review not found");

            // Ensure user can only update their own review
            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You can only update your own reviews");

            // Validate rating
            if (updateReviewDto.Rating < 1 || updateReviewDto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            review.Rating = updateReviewDto.Rating;
            review.Comment = updateReviewDto.Comment;

            await _context.SaveChangesAsync();

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<bool> DeleteReviewAsync(int id, string userId)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return false;

            // Ensure user can only delete their own review
            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own reviews");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BookId == bookId)
                .ToListAsync();

            if (!reviews.Any())
                return 0.0;

            return reviews.Average(r => r.Rating);
        }

        public async Task<int> GetReviewCountAsync(int bookId)
        {
            return await _context.Reviews
                .Where(r => r.BookId == bookId)
                .CountAsync();
        }

        public async Task<bool> HasUserReviewedBookAsync(string userId, int bookId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.BookId == bookId);
        }
    }
}