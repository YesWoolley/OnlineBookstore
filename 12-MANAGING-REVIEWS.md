# Section 12: Managing Reviews

Welcome to the review management phase! In this section, we'll create the backend services for review operations. Reviews are essential for user engagement and book discovery, allowing users to rate and comment on books they've read.

---

## 🎯 What You'll Learn

- How to create backend services for review operations
- How to implement rating validation and calculations
- How to handle user-specific review permissions
- How to manage review moderation and quality control
- How to implement review aggregation for book ratings
- How to structure services for user-generated content

---

## 🏗️ Step 1: Create Review DTOs

### **Create DTOs/ReviewDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends review data TO clients with user and book names
    public class ReviewDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Input DTO: Receives review data FROM clients for creation
    public class CreateReviewDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
    }

    // Input DTO: Receives review data FROM clients for updates
    public class UpdateReviewDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
    }
}
```

---

## 🎮 Step 2: Create Review Service

### **Create Services/IReviewService.cs:**
```csharp
using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
        Task<IEnumerable<ReviewDto>> GetReviewsByBookAsync(int bookId);
        Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(string userId);
        Task<ReviewDto?> GetReviewByIdAsync(int id);
        Task<ReviewDto> CreateReviewAsync(string userId, CreateReviewDto createReviewDto);
        Task<ReviewDto> UpdateReviewAsync(int id, string userId, UpdateReviewDto updateReviewDto);
        Task<bool> DeleteReviewAsync(int id, string userId);
        Task<double> GetAverageRatingAsync(int bookId);
        Task<int> GetReviewCountAsync(int bookId);
        Task<bool> HasUserReviewedBookAsync(string userId, int bookId);
    }
}
```

### **Create Services/ReviewService.cs:**
```csharp
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
```

---

## 🎮 Step 3: Create Review Controller

### **Understanding Interface vs Implementation**

Before we create the controller, let's understand why we use `IReviewService` instead of `ReviewService` directly:

#### **🔍 Interface (Contract)**
```csharp
// IReviewService.cs - Just defines WHAT methods exist (empty contract)
public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
    Task<ReviewDto?> GetReviewByIdAsync(int id);
    Task<ReviewDto> CreateReviewAsync(string userId, CreateReviewDto createReviewDto);
    // ... other method signatures (no implementation)
}
```

#### **🏗️ Implementation (Real Code)**
```csharp
// ReviewService.cs - Contains the REAL implementation
public class ReviewService : IReviewService  // ← Implements the interface
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ReviewService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // REAL implementation of the interface method
    public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
    {
        var reviews = await _context.Reviews
            .Include(r => r.Book)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }
}
```

#### **🔗 How They Connect**
```csharp
// Program.cs - Tells DI container: "When someone asks for IReviewService, give them ReviewService"
builder.Services.AddScoped<IReviewService, ReviewService>();
//     ↑ Interface    ↑ Concrete Implementation
```

#### **🎯 Why This Pattern?**
- **Controller doesn't know** about `ReviewService` implementation details
- **Easy to swap** implementations (e.g., for testing or caching)
- **Follows SOLID principles** (Dependency Inversion)
- **Better testability** - can mock the interface easily

### **Create Controllers/ReviewsController.cs:**
```csharp
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
```

#### **🔄 What Actually Happens When Controller Uses Interface**

```csharp
// 1. Controller asks for IReviewService
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService; // ← Interface type

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService; // ← Gets REAL ReviewService instance
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()
    {
        // 2. This calls the REAL implementation in ReviewService
        var reviews = await _reviewService.GetAllReviewsAsync();
        return Ok(reviews);
    }
}
```

**The Process:**
1. **Controller asks for** `IReviewService` (interface)
2. **DI Container looks up** the mapping: `IReviewService` → `ReviewService`
3. **DI Container creates** a new `ReviewService` instance
4. **DI Container injects** the `ReviewService` instance into the controller
5. **Controller calls** methods on the interface, but gets the real implementation

**Interface = "What" (contract)**
**Implementation = "How" (real code)**

---

## ⚙️ Step 4: Update AutoMapper Profile

### **Update Mapppings/AutoMapperProfile.cs:**
```csharp
using AutoMapper;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;

namespace OnlineBookstore.Mapppings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Existing mappings...

            // Review mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<CreateReviewDto, Review>();
            CreateMap<UpdateReviewDto, Review>();
        }
    }
}
```

---

## ⚙️ Step 5: Register Service in Program.cs

### **Update Program.cs:**
```csharp
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using OnlineBookstore.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Register Services
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IReviewService, ReviewService>(); // ----add

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Seed the database
await DbInitializer.SeedAsync(app);

app.Run();
```

---

## 🧪 Step 6: Test Your Review Management

1. **Start the backend application:**
   ```bash
   # Backend
   dotnet run
   ```

2. **Test API endpoints using Postman or curl:**
   ```bash
# Get all reviews
GET https://localhost:7273/api/reviews

# Get specific review
GET https://localhost:7273/api/reviews/1

# Get reviews by book
GET https://localhost:7273/api/reviews/book/1

# Get user reviews
GET https://localhost:7273/api/reviews/user/current

# Get average rating for book
GET https://localhost:7273/api/reviews/book/1/average-rating

# Get review count for book
GET https://localhost:7273/api/reviews/book/1/review-count

# Check if user has reviewed book
GET https://localhost:7273/api/reviews/book/1/has-reviewed

# Create review
POST https://localhost:7273/api/reviews
Content-Type: application/json

{
  "bookId": 1,
  "rating": 5,
  "comment": "Excellent book! Highly recommended."
}

# Update review
PUT https://localhost:7273/api/reviews/1
Content-Type: application/json

{
  "rating": 4,
  "comment": "Very good book, but could be better."
}

# Delete review
DELETE https://localhost:7273/api/reviews/1
   ```

3. **Test the Swagger UI:**
   - Navigate to `https://localhost:7273/swagger`
   - Test all the review endpoints directly from the browser

---

## 🏆 Best Practices

### **Review Management Best Practices:**
- **Rating validation** (1-5 stars only)
- **One review per user per book** (prevent duplicates)
- **User authorization** (users can only modify their own reviews)
- **Content moderation** (filter inappropriate comments)
- **Review aggregation** (calculate average ratings)

### **User-Generated Content Best Practices:**
- **Input validation** for ratings and comments
- **Content length limits** to prevent abuse
- **User authentication** for review creation
- **Review quality** monitoring
- **Spam prevention** measures

### **Review System Best Practices:**
- **Real-time rating updates** for books
- **Review helpfulness** voting
- **Review sorting** (newest, highest rated, most helpful)
- **Review reporting** for inappropriate content
- **Review analytics** for insights

---

## ✅ What You've Accomplished

- ✅ Created **review service** with complete CRUD operations
- ✅ **Implemented rating validation** (1-5 stars)
- ✅ **Added user authorization** (own reviews only)
- ✅ **Created review aggregation** (average ratings, counts)
- ✅ **Implemented duplicate prevention** (one review per user per book)
- ✅ **Added comprehensive API endpoints** for review management
- ✅ **Set up proper validation** for ratings and comments
- ✅ **Created review analytics** (average rating, review count)

---

## 🚀 Next Steps

Your review management backend is now complete! This completes all the core backend services for the e-bookstore application.

**You've successfully created a complete review management system with user authorization and rating aggregation. Great job!**

---

**🎉 Congratulations! You've now completed all the major backend services:**
- ✅ Authors, Publishers, Categories
- ✅ Books with complex relationships
- ✅ Shopping Cart and Orders
- ✅ Reviews with user engagement

**Your e-bookstore backend is now feature-complete!** 