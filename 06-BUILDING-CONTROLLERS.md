# Section 6: Building and Managing Controllers (API Endpoints)

Welcome to the API development phase! In this section, we'll create RESTful API controllers for our Online Bookstore, implement proper data transfer objects (DTOs), set up AutoMapper for object mapping, and establish best practices for API design.

---

## 📝 **Important Note: This is the Basic Approach**

**This section shows the basic controller approach (direct database access).** In Section 7, we'll introduce the **service layer pattern** which is the recommended approach for production applications.

**Section 6 vs Section 7:**
- **Section 6**: Controllers directly access database (simpler, good for learning)
- **Section 7**: Controllers use services (better architecture, production-ready)

---

## 🔄 **Service vs Controller Return Types**

**Quick Rule:** 
- **Services** return business objects (DTOs, entities)
- **Controllers** return HTTP responses (`IActionResult`)

---

## 🎯 What You'll Learn

- How to create RESTful API controllers
- How to implement DTOs for data transfer
- How to use AutoMapper for object mapping
- How to handle HTTP status codes and responses
- Best practices for API design and error handling

## 📋 **Important: Follow Steps in Order**

**The steps must be completed in this exact order:**
1. **Step 1**: Install packages (AutoMapper)
2. **Step 2**: Understand controllers (concepts)
3. **Step 3**: Create DTOs (data structures)
4. **Step 4**: Set up AutoMapper (object mapping)
5. **Step 5**: Create controllers (API endpoints)

**Why this order matters:** Controllers depend on AutoMapper, and AutoMapper depends on DTOs. Skipping steps will cause compilation errors!

---

## 📦 Step 1: Install Required NuGet Packages

First, let's install AutoMapper for object mapping:

### **Via Package Manager Console:**
```powershell
Install-Package AutoMapper.Extensions.Microsoft.DependencyInjection
```

### **Via .NET CLI:**
```bash
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

---

## 🏗️ Step 2: Understanding API Controllers

### **🎯 What are API Controllers?**

API Controllers are like **"waiters"** in a restaurant - they take requests from clients (frontend) and serve responses. They handle HTTP requests and return data in JSON format.

### **🔧 How They Work:**
```
Frontend (React) → HTTP Request → API Controller → Database → Response → Frontend
```

### **📋 Controller Responsibilities:**
- ✅ **Receive HTTP requests** (GET, POST, PUT, DELETE)
- ✅ **Validate input data**
- ✅ **Call business logic** (services)
- ✅ **Return appropriate responses**
- ✅ **Handle errors gracefully**

---

## 📝 Step 3: Create DTOs (Data Transfer Objects)

Create a new folder called `DTOs` in your `OnlineBookstore` project. DTOs are like **"data packages"** that define what data goes in and out of your API.

**Why DTOs?** DTOs are needed to **hide sensitive database fields** (like internal IDs, audit fields, or confidential data) from your API responses while only exposing the clean, relevant data that clients actually need. **Validation Tip:** Put validation attributes in DTOs (not models) for API-specific rules and flexibility.

**Where to Put Validation:** Always put validation attributes in your DTOs rather than your database models. This gives you API-specific validation rules, allows different validation for Create vs Update operations, and keeps your database models clean and focused on structure rather than business rules.

**Validation vs Nullable Types:** Database models need C# nullable reference types (`string Title { get; set; } = null!` or `string? Description { get; set; }`) for compile-time null safety, while DTOs need validation attributes (`[Required]`, `[StringLength]`) for runtime API validation. Models focus on structure, DTOs focus on business rules.

**DTO Design Pattern:** Input DTOs use foreign key IDs (`AuthorId`, `PublisherId`, `CategoryId`) for creating relationships, while response DTOs use human-readable names (`AuthorName`, `PublisherName`, `CategoryName`) for display. This follows the standard REST API pattern where clients work with IDs but receive readable data.

**Collection Inclusion Strategy:** Include collections in DTOs when users need complete views (like OrderDto with OrderItems), but avoid collections when users want summaries (like AuthorDto with just BookCount). This balances performance with user-friendly design - show users what they need, when they need it.

**DTO Purpose Clarification:** Create = What users SEND to create new data (minimal input), Response = What users SEE after creation (complete output), Update = What users SEND to modify existing data. The pattern is "ask for little, give back much" - users provide essential data, API returns full context with calculated values.

**The Pattern Explained:**
- **Create DTOs (Minimal Input):** Users send only essential data (BookId, Quantity), system handles the rest (IDs, calculations)
- **Response DTOs (Complete Output):** Users receive full context with auto-generated values (Id, BookTitle, TotalPrice) and calculated fields
- **Update DTOs (Modification Input):** Users send only the fields they want to change, system preserves existing data

**Example Code Pattern:**
```csharp
// Create DTOs (Minimal Input)
public class CreateOrderItemDto
{
    public int BookId { get; set; }      // ← User sends this
    public int Quantity { get; set; }     // ← User sends this
}

// Response DTOs (Complete Output)  
public class OrderItemDto
{
    public int Id { get; set; }                    // ← Auto-generated
    public string BookTitle { get; set; } = null!; // ← Auto-loaded
    public decimal TotalPrice { get; set; }        // ← Auto-calculated
    public int Quantity { get; set; }               // ← What user sent
}

// Update DTOs (Modification Input)
public class UpdateOrderItemDto
{
    public int Quantity { get; set; }     // ← User sends this
}
```

### **Create DTOs/BookDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends clean data TO clients (validation ensures data integrity)
    public class BookDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = null!;
        
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]
        public int StockQuantity { get; set; }
        
        [Url(ErrorMessage = "Cover image URL must be a valid URL")]
        public string? CoverImageUrl { get; set; }
        
        // Related data (AuthorName is created manually but later mapped to Book.Author.Name via AutoMapper)
        // Source: Author author, Publisher publisher, Category category as listed in Book model
        [Required]
        [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters")]
        public string AuthorName { get; set; } = null!;
        
        [Required]
        [StringLength(100, ErrorMessage = "Publisher name cannot exceed 100 characters")]
        public string PublisherName { get; set; } = null!;
        
        [Required]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters")]
        public string CategoryName { get; set; } = null!;
        
        // Review summary (for quick display without loading all reviews)
        // These are calculated properties that show review statistics
        [Range(0, int.MaxValue, ErrorMessage = "Review count cannot be negative")]
        public int ReviewCount { get; set; }
        
        [Range(0.0, 5.0, ErrorMessage = "Average rating must be between 0 and 5")]
        public double AverageRating { get; set; }
    }

    // Detailed response DTO: Sends complete book data TO clients including all reviews
    public class BookDetailDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = null!;
        
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]
        public int StockQuantity { get; set; }
        
        [Url(ErrorMessage = "Cover image URL must be a valid URL")]
        public string? CoverImageUrl { get; set; }
        
        // Related data
        [Required]
        [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters")]
        public string AuthorName { get; set; } = null!;
        
        [Required]
        [StringLength(100, ErrorMessage = "Publisher name cannot exceed 100 characters")]
        public string PublisherName { get; set; } = null!;
        
        [Required]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters")]
        public string CategoryName { get; set; } = null!;
        
        // Review data (complete review information for detailed views)
        [Range(0, int.MaxValue, ErrorMessage = "Review count cannot be negative")]
        public int ReviewCount { get; set; }
        
        [Range(0.0, 5.0, ErrorMessage = "Average rating must be between 0 and 5")]
        public double AverageRating { get; set; }
        
        public ICollection<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    }

    // Input DTO: Receives data FROM clients for creating new books (validation protects API)
    public class CreateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]
        public int StockQuantity { get; set; }
        
        [Url]
        public string? CoverImageUrl { get; set; }
        
        // Foreign key IDs (AuthorId is created manually but later mapped to Book.Author.Id via AutoMapper)
        [Required]
        public int AuthorId { get; set; }
        
        // Foreign key IDs (PublisherId is created manually but later mapped to Book.Publisher.Id via AutoMapper)
        [Required]
        public int PublisherId { get; set; }
        
        // Foreign key IDs (CategoryId is created manually but later mapped to Book.Category.Id via AutoMapper)
        [Required]
        public int CategoryId { get; set; }
    }

    // Input DTO: Receives data FROM clients for updating existing books (validation ensures data integrity)
    public class UpdateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]
        public int StockQuantity { get; set; }
        
        [Url]
        public string? CoverImageUrl { get; set; }
        
        // Foreign key IDs (AuthorId is created manually but later mapped to Book.Author.Id via AutoMapper)
        [Required]
        public int AuthorId { get; set; }
        
        // Foreign key IDs (PublisherId is created manually but later mapped to Book.Publisher.Id via AutoMapper)
        [Required]
        public int PublisherId { get; set; }
        
        // Foreign key IDs (CategoryId is created manually but later mapped to Book.Category.Id via AutoMapper)
        [Required]
        public int CategoryId { get; set; }
    }
}
```

### **Create DTOs/AuthorDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends author data TO clients with book count for display
    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Biography { get; set; }
        // Calculated property (BookCount is created manually but later mapped to Author.Books.Count via AutoMapper)
        // No validation needed: Response DTOs don't need validation since data comes from database calculation, not user input
        public int BookCount { get; set; }
    }

    // Input DTO: Receives author data FROM clients for creation (validation prevents empty names)
    public class CreateAuthorDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        
        [StringLength(1000)]
        public string? Biography { get; set; }
    }

    // Input DTO: Receives author data FROM clients for updates (validation maintains data quality)
    public class UpdateAuthorDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        
        [StringLength(1000)]
        public string? Biography { get; set; }
    }
}
```

### **Create DTOs/CategoryDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends category data TO clients with book count for filtering
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int BookCount { get; set; }
    }

    // Input DTO: Receives category data FROM clients for creation (validation ensures unique names)
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
    }

    // Input DTO: Receives category data FROM clients for updates (validation prevents duplicate names)
    public class UpdateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
    }
}
```

### **Create DTOs/PublisherDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends publisher data TO clients with book count for display
    public class PublisherDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        // Calculated property (BookCount is created manually but later mapped to Publisher.Books.Count via AutoMapper)
        // No validation needed: Response DTOs don't need validation since data comes from database calculation, not user input
        public int BookCount { get; set; }
    }

    // Input DTO: Receives publisher data FROM clients for creation (validation prevents empty names)
    public class CreatePublisherDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        
        [StringLength(1000)]
        public string? Description { get; set; }
    }

    // Input DTO: Receives publisher data FROM clients for updates (validation maintains data quality)
    public class UpdatePublisherDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        
        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
```

### **Create DTOs/ReviewDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends review data TO clients with user and book names for display
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

    // Input DTO: Receives review data FROM clients for creation (validation ensures valid ratings)
    public class CreateReviewDto
    {
        [Required]
        public int BookId { get; set; }
        
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        
        [StringLength(1000)]
        public string? Comment { get; set; }
    }

    // Input DTO: Receives review data FROM clients for updates (validation maintains review quality)
    public class UpdateReviewDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        
        [StringLength(1000)]
        public string? Comment { get; set; }
    }
}
```

### **Create DTOs/OrderDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends order data TO clients with user name and order items for display
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    // Input DTO: Receives order data FROM clients for creation (validation ensures valid order)
    public class CreateOrderDto
    {
        [Required]
        public string ShippingAddress { get; set; } = null!;
        
        [Required]
        public string OrderStatus { get; set; } = null!;
        
        [Required]
        public ICollection<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }

    // Input DTO: Receives order data FROM clients for updates (validation maintains order integrity)
    public class UpdateOrderDto
    {
        [Required]
        public string ShippingAddress { get; set; } = null!;
        
        [Required]
        public string OrderStatus { get; set; } = null!;
    }
}
```

### **Create DTOs/OrderItemDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends order item data TO clients with book details for display
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // Input DTO: Receives order item data FROM clients for creation (validation ensures valid quantities)
    public class CreateOrderItemDto
    {
        [Required]
        public int BookId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    // Input DTO: Receives order item data FROM clients for updates (validation maintains order integrity)
    public class UpdateOrderItemDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
```

### **Create DTOs/ShoppingCartItemDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends cart item data TO clients with book details for display
    public class ShoppingCartItemDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public decimal BookPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // Input DTO: Receives cart item data FROM clients for creation (validation ensures valid quantities)
    public class CreateShoppingCartItemDto
    {
        [Required]
        public int BookId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    // Input DTO: Receives cart item data FROM clients for updates (validation maintains cart integrity)
    public class UpdateShoppingCartItemDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set;         }
    }
}
```

---

## 🔧 Step 4: Set Up AutoMapper (Required Before Controllers)

**⚠️ CRITICAL:** This step must be completed before creating controllers. Controllers use `IMapper` which is configured here.

Create a new file `Mappings/AutoMapperProfile.cs`:

```csharp
using AutoMapper;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;

namespace OnlineBookstore.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        { 
            // Book mappings
            CreateMap<Book, BookDto>()
                // src refers to the first parameter in the CreateMap method.
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 
                    src.Reviews != null && src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0));

            CreateMap<Book, BookDetailDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 
                    src.Reviews != null && src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews ?? new List<Review>()))
                // This ensures that even when a book has no reviews in the database (null), the API will return an empty array [] instead of null, making it easier for frontend applications to handle.

            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();

            // Author mappings
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books != null ? src.Books.Count : 0));

            CreateMap<CreateAuthorDto, Author>();
            CreateMap<UpdateAuthorDto, Author>();

            // Category mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books != null ? src.Books.Count : 0));

            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            // Publisher mappings
            CreateMap<Publisher, PublisherDto>()
                .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books != null ? src.Books.Count : 0));

            CreateMap<CreatePublisherDto, Publisher>();
            CreateMap<UpdatePublisherDto, Publisher>();

            // Review mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<CreateReviewDto, Review>();
            CreateMap<UpdateReviewDto, Review>();

            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<CreateOrderDto, Order>();
            CreateMap<UpdateOrderDto, Order>();

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

            CreateMap<CreateOrderItemDto, OrderItem>();
            CreateMap<UpdateOrderItemDto, OrderItem>();

            // ShoppingCartItem mappings
            CreateMap<ShoppingCartItem, ShoppingCartItemDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.BookPrice, opt => opt.MapFrom(src => src.Book.Price))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.Book.Price));

            CreateMap<CreateShoppingCartItemDto, ShoppingCartItem>();
            CreateMap<UpdateShoppingCartItemDto, ShoppingCartItem>();
        }
    }
}
```

---


## 🎮 Step 5: Create Controllers (After AutoMapper Setup)

**✅ PREREQUISITE:** Make sure you've completed Step 4 (AutoMapper setup) before creating controllers. Controllers use `IMapper` which is configured in Step 4.

**Collection Interface Notes:**
- **IEnumerable = "Can loop through"** (read-only collection)
- **ICollection = "Can loop through + add/remove/count"** (read-write collection)

Create a new folder called `Controllers` and add the following controllers:

### **BooksController.cs:**

```csharp
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
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/books
        [HttpGet]
        // IEnumerable = "Can loop through" (read-only collection)
        // ICollection = "Can loop through + add/remove/count"
        // ActionResult = "The result of an HTTP request with proper status codes"
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var books = await _context.Books  // ← Get books from database
                    .Include(b => b.Author)       // ← Load related author data
                    .Include(b => b.Publisher)    // ← Load related publisher data
                    .Include(b => b.Category)     // ← Load related category data
                    .Include(b => b.Reviews)      // ← Load related review data for count/rating
                    .ToListAsync();               // ← Execute query and return list

                // Input: [{ Id: 1, AuthorId: 5, PublisherId: 3 }]
                // AutoMapper: Applies rules (AuthorId → AuthorName, etc.)
                // Output: [{ Id: 1, AuthorName: "J.K. Rowling", PublisherName: "Scholastic" }]
                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving books", error = ex.Message });
            }
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Publisher)
                    .Include(b => b.Category)
                    .Include(b => b.Reviews)
                    .FirstOrDefaultAsync(b => b.Id == id);
                    //           ↑           ↑        ↑
                    //           |           |        |
                    //           |           |        └── parameter from URL
                    //           |           └── lambda expression (filter condition)
                    //           └── async method (non-blocking)

                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                var bookDto = _mapper.Map<BookDto>(book);
                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the book", error = ex.Message });
            }
        }

        // GET: api/books/5/detail
        // Returns complete book information including all reviews (for detailed book pages)
        [HttpGet("{id}/detail")]
        public async Task<ActionResult<BookDetailDto>> GetBookDetail(int id)  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Publisher)
                    .Include(b => b.Category)
                    .Include(b => b.Reviews!)
                        .ThenInclude(r => r.User)  // ← Load user data for each review
                    .FirstOrDefaultAsync(b => b.Id == id);
                    // Note: ! is needed on Reviews because ThenInclude() can't chain through nullable collections
                    // Simple Include() works fine with nullable properties, but ThenInclude() requires ! operator

                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                var bookDetailDto = _mapper.Map<BookDetailDto>(book);
                return Ok(bookDetailDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the book details", error = ex.Message });
            }
        }

        // POST: api/books
        [HttpPost]
        public async Task<IActionResult> CreateBook(CreateBookDto createBookDto)  // ← POST: Use IActionResult (different success responses)
        {
            try
            {
                // Apply validation in POST and PUT methods, NOT in GET and DELETE.
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(createBookDto);  // ← Convert DTO to database entity
                _context.Books.Add(book);                     // ← Add to database context (not saved yet)
                await _context.SaveChangesAsync();             // ← Save to database and get auto-generated ID

                // Reload the book with related data for the response
                await _context.Entry(book)                    // ← Get EF tracking entry for the book
                    .Reference(b => b.Author)                 // ← Load single Author navigation property
                    .LoadAsync();                             // ← Execute async query to fetch Author data
                await _context.Entry(book)                    // ← Get EF tracking entry for the book
                    .Reference(b => b.Publisher)              // ← Load single Publisher navigation property
                    .LoadAsync();                             // ← Execute async query to fetch Publisher data
                await _context.Entry(book)                    // ← Get EF tracking entry for the book
                    .Reference(b => b.Category)               // ← Load single Category navigation property
                    .LoadAsync();                             // ← Execute async query to fetch Category data
                await _context.Entry(book)                    // ← Get EF tracking entry for the book
                    .Collection(b => b.Reviews!)              // ← Load collection of Reviews navigation property
                    .LoadAsync();                             // ← Execute async query to fetch Reviews data
                // why maps second: User input (createdto) → Map 1 → Database entity(book model)
                // database entity(book model) → Map 2 → API response(bookdto)
                var bookDto = _mapper.Map<BookDto>(book);
                // Status: 201 Created
                // Header: Location: /api/books/5
                // Body: Complete book data
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, bookDto);  // ← POST: Returns 201 Created + Location header
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the book", error = ex.Message });
            }
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        // IActionResult           // ← Can return any HTTP response
        // ActionResult<BookDto>   // ← Can return BookDto OR error responses
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDto updateBookDto)  // ← PUT: Use IActionResult (different success responses)
        {
            try
            {
                // Apply validation in POST and PUT methods, NOT in GET and DELETE.
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                _mapper.Map(updateBookDto, book);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the book", error = ex.Message });
            }
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)  // ← DELETE: Use IActionResult (different success responses)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the book", error = ex.Message });
            }
        }

        // GET: api/books/search?query=harry
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDto>>> SearchBooks([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { message = "Search query is required" });
                }

                var books = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Publisher)
                    .Include(b => b.Category)
                    .Include(b => b.Reviews)
                    .Where(b => b.Title.Contains(query) || 
                               b.Author.Name.Contains(query) || 
                               b.Category.Name.Contains(query))
                    .ToListAsync();

                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching books", error = ex.Message });
            }
        }
    }
}
```

---

## 🎮 Step 6: Create Authors Controller

Create `Controllers/AuthorsController.cs`:

```csharp
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
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AuthorsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var authors = await _context.Authors
                    .Include(a => a.Books)
                    .ToListAsync();

                var authorDtos = _mapper.Map<IEnumerable<AuthorDto>>(authors);
                return Ok(authorDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving authors", error = ex.Message });
            }
        }

        // GET: api/authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthor(int id)  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var author = await _context.Authors
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (author == null)
                {
                    return NotFound(new { message = "Author not found" });
                }

                var authorDto = _mapper.Map<AuthorDto>(author);
                return Ok(authorDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the author", error = ex.Message });
            }
        }

        // POST: api/authors
        [HttpPost]
        public async Task<IActionResult> CreateAuthor(CreateAuthorDto createAuthorDto)  // ← POST: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(createAuthorDto);
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();

                // For new authors, BookCount will be 0 (no books yet)
                var authorDto = _mapper.Map<AuthorDto>(author);
                authorDto.BookCount = 0; // Explicitly set for new authors
                
                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, authorDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the author", error = ex.Message });
            }
        }

        // PUT: api/authors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, UpdateAuthorDto updateAuthorDto)  // ← PUT: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    return NotFound(new { message = "Author not found" });
                }

                _mapper.Map(updateAuthorDto, author);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the author", error = ex.Message });
            }
        }

        // DELETE: api/authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)  // ← DELETE: Use IActionResult (different success responses)
        {
            try
            {
                var author = await _context.Authors
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (author == null)
                {
                    return NotFound(new { message = "Author not found" });
                }

                if (author.Books != null && author.Books.Any())
                {
                    return BadRequest(new { message = "Cannot delete author with existing books" });
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the author", error = ex.Message });
            }
        }

        // GET: api/authors/search?query=tolkien
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> SearchAuthors([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { message = "Search query cannot be empty" });
                }

                var authors = await _context.Authors
                    .Include(a => a.Books)
                    .Where(a => a.Name.Contains(query) || (a.Biography != null && a.Biography.Contains(query)))
                    .ToListAsync();

                if (!authors.Any())
                {
                    return NotFound(new { message = "No authors found matching the search criteria" });
                }

                var authorDtos = _mapper.Map<IEnumerable<AuthorDto>>(authors);
                return Ok(authorDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching for authors", error = ex.Message });
            }
        }
    }
}
```

---

## 🎮 Step 7: Create Categories Controller

Create `Controllers/CategoriesController.cs`:

```csharp
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
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.Books)
                    .ToListAsync();

                var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
                return Ok(categoryDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving categories", error = ex.Message });
            }
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Books)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the category", error = ex.Message });
            }
        }

        // POST: api/categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)  // ← POST: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = _mapper.Map<Category>(createCategoryDto);
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the category", error = ex.Message });
            }
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)  // ← PUT: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                _mapper.Map(updateCategoryDto, category);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the category", error = ex.Message });
            }
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)  // ← DELETE: Use IActionResult (different success responses)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Books)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                if (category.Books != null && category.Books.Any())
                {
                    return BadRequest(new { message = "Cannot delete category with existing books" });
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the category", error = ex.Message });
            }
        }
    }
}
```

---

## 🎮 Step 8: Create Publishers Controller
```csharp
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
    public class PublishersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PublishersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/publishers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherDto>>> GetPublishers()  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var publishers = await _context.Publishers
                    .Include(p => p.Books)
                    .ToListAsync();

                var publisherDtos = _mapper.Map<IEnumerable<PublisherDto>>(publishers);
                return Ok(publisherDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving publishers", error = ex.Message });
            }
        }

        // GET: api/publishers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherDto>> GetPublisher(int id)  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var publisher = await _context.Publishers
                    .Include(p => p.Books)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (publisher == null)
                {
                    return NotFound(new { message = "Publisher not found" });
                }

                var publisherDto = _mapper.Map<PublisherDto>(publisher);
                return Ok(publisherDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the publisher", error = ex.Message });
            }
        }

        // POST: api/publishers
        [HttpPost]
        public async Task<IActionResult> CreatePublisher(CreatePublisherDto createPublisherDto)  // ← POST: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var publisher = _mapper.Map<Publisher>(createPublisherDto);
                _context.Publishers.Add(publisher);
                await _context.SaveChangesAsync();

                var publisherDto = _mapper.Map<PublisherDto>(publisher);
                return CreatedAtAction(nameof(GetPublisher), new { id = publisher.Id }, publisherDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the publisher", error = ex.Message });
            }
        }

        // PUT: api/publishers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublisher(int id, UpdatePublisherDto updatePublisherDto)  // ← PUT: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var publisher = await _context.Publishers.FindAsync(id);
                if (publisher == null)
                {
                    return NotFound(new { message = "Publisher not found" });
                }

                _mapper.Map(updatePublisherDto, publisher);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the publisher", error = ex.Message });
            }
        }

        // DELETE: api/publishers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)  // ← DELETE: Use IActionResult (different success responses)
        {
            try
            {
                var publisher = await _context.Publishers
                    .Include(p => p.Books)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (publisher == null)
                {
                    return NotFound(new { message = "Publisher not found" });
                }

                if (publisher.Books != null && publisher.Books.Any())
                {
                    return BadRequest(new { message = "Cannot delete publisher with existing books" });
                }

                _context.Publishers.Remove(publisher);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the publisher", error = ex.Message });
            }
        }
    }
}
```

## 🎮 Step 9: Create Reviews Controller
```csharp
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
```

## 🎮 Step 10: Create Orders Controller
```csharp
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
                    .Collection(o => o.OrderItems)
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
                    .Include(o => o.OrderItems!)
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
```

## 🎮 Step 11: Create OrderItems Controller
```csharp
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
```

## 🎮 Step 12: Create ShoppingCartItems Controller
```csharp
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
```

---

## ⚙️ Step 13: Configure Program.cs for ASP.NET Core 9

Update your `Program.cs` to include all necessary services for your OnlineBookstore application:

```csharp
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configure Authentication
builder.Services.AddAuthentication(options => 
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Memory Cache and Session
builder.Services.AddMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    // Configure Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineBookstore API V1");
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

// Seed the database
DbInitializer.SeedAsync(app).Wait();

app.Run();
```

---

## 🧪 Step 14: Test Your API

1. **Run the application:**
   ```bash
   # For HTTP (default)
   dotnet run
   
   # For HTTPS
   dotnet run --launch-profile https
   ```

2. **Open Swagger UI:**
   - **HTTP:** Navigate to `http://localhost:5117/swagger`
   - **HTTPS:** Navigate to `https://localhost:7273/swagger`
   - Test the endpoints interactively

3. **Test with Postman or curl:**

### **Get All Books:**
```bash
# HTTP
GET http://localhost:5117/api/books

# HTTPS
GET https://localhost:7273/api/books
```

### **Get Book by ID:**
```bash
# HTTP
GET http://localhost:5117/api/books/1

# HTTPS
GET https://localhost:7273/api/books/1
```

### **Create New Book:**
```bash
# HTTP
POST http://localhost:5117/api/books
Content-Type: application/json

# HTTPS
POST https://localhost:7273/api/books
Content-Type: application/json

{
  "title": "The Great Gatsby",
  "description": "A classic American novel",
  "price": 15.99,
  "stockQuantity": 25,
  "authorId": 1,
  "publisherId": 1,
  "categoryId": 1
}
```

### **Update Book:**
```bash
# HTTP
PUT http://localhost:5117/api/books/1
Content-Type: application/json

# HTTPS
PUT https://localhost:7273/api/books/1
Content-Type: application/json

{
  "title": "The Great Gatsby (Updated)",
  "description": "A classic American novel about the Jazz Age",
  "price": 19.99,
  "stockQuantity": 30,
  "authorId": 1,
  "publisherId": 1,
  "categoryId": 1
}
```

### **Delete Book:**
```bash
# HTTP
DELETE http://localhost:5117/api/books/1

# HTTPS
DELETE https://localhost:7273/api/books/1
```

### **Search Books:**
```bash
# HTTP
GET http://localhost:5117/api/books/search?query=harry

# HTTPS
GET https://localhost:7273/api/books/search?query=harry
```

---

## 🏆 Best Practices

### **Return Type Guidelines:**
- **Use `ActionResult<T>` for:**
  - GET methods - You know what you're returning
  - Simple operations - Success = return data, Failure = return error
- **Use `IActionResult` for:**
  - POST/PUT/DELETE - Different success responses
  - Complex operations - Multiple possible success types

### **Validation Guidelines:**
- **Apply validation in POST and PUT methods, NOT in GET and DELETE**
- **Why This Pattern:**
  - **POST/PUT - User Input:**
    - Receives data from client (DTOs)
    - Data needs validation (`[Required]`, `[StringLength]`, etc.)
    - `ModelState` contains validation results
  - **GET/DELETE - No User Input:**
    - GET: Just reads data (no input to validate)
    - DELETE: Just uses ID (no complex input to validate)

### **Response Guidelines:**
- **POST (Create):** Use `CreatedAtAction()` - Returns 201 Created + Location header
- **GET (Read):** Use `Ok()` - Returns 200 OK with data
- **PUT (Update):** Use `NoContent()` - Returns 204 NoContent
- **DELETE (Remove):** Use `NoContent()` - Returns 204 NoContent
- **Why This Pattern:**
  - **CreatedAtAction Purpose:** 201 Created = "I created a new resource", Location header = "Here's where to find it"
  - **Other Operations:** GET = Resource already exists, PUT = Resource already exists, DELETE = Resource existed, now it's gone

### **API Design:**
- Use **RESTful conventions** (GET, POST, PUT, DELETE)
- Return **appropriate HTTP status codes**
- Include **error messages** in responses
- Use **DTOs** for data transfer
- Implement **input validation**

### **Error Handling:**
- Use **try-catch blocks** for database operations
- Return **meaningful error messages**
- Log **exceptions** for debugging
- Use **HTTP status codes** correctly

### **Performance:**
- Use **Include()** for related data
- Implement **pagination** for large datasets
- Use **async/await** for database operations
- Consider **caching** for frequently accessed data

---

## ✅ What You've Accomplished

- ✅ Created RESTful API controllers
- ✅ Implemented DTOs for data transfer
- ✅ Set up AutoMapper for object mapping
- ✅ Added proper error handling
- ✅ Implemented input validation
- ✅ Created search functionality
- ✅ Set up Swagger documentation

---

## 🚀 Next Steps

Your API is now ready! In the next section, we'll start building the React frontend and connecting it to these API endpoints.

**You've successfully created a robust API foundation. Great job!**

---

**Next up:**
- [Section 7: Understanding UI Design](./07-UNDERSTANDING-UI-DESIGN.md) 