# Section 10: Managing Book Data

Welcome to the book management phase! In this section, we'll create the backend services for book operations using simple services with direct DbContext access. Books are more complex than categories and publishers because they have multiple relationships (Author, Publisher, Category, Reviews).

---

## 🎯 What You'll Learn

- How to create backend services for book operations with complex relationships
- How to implement CRUD operations (Create, Read, Update, Delete) for books
- How to handle multiple entity relationships (Author, Publisher, Category)
- How to implement search functionality across multiple fields
- How to manage review data and ratings
- How to structure services for complex entities

---

## 🏗️ Step 1: Create Book DTOs

### **Create DTOs/BookDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends clean data TO clients with related entity names
    // Note: No validation attributes needed on response DTOs (server controls the data)
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? CoverImageUrl { get; set; }

        // Related entity names (mapped from relationships)
        public string AuthorName { get; set; } = null!;
        public string PublisherName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;

        // Review summary (calculated properties)
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
    }

    // Detailed response DTO: Includes complete review data
    public class BookDetailDto : BookDto
    {
        public ICollection<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    }

    // Input DTO: Receives data FROM clients for creating new books
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

        // Foreign key IDs for relationships
        [Required]
        public int AuthorId { get; set; }

        [Required]
        public int PublisherId { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }

    // Input DTO: Receives data FROM clients for updating existing books
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

        // Foreign key IDs for relationships
        [Required]
        public int AuthorId { get; set; }

        [Required]
        public int PublisherId { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
```

---

## 🎮 Step 2: Create Book Service

### **Create Services/IBookService.cs:**
```csharp
using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDetailDto?> GetBookDetailByIdAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookDto createBookDto);
        Task<BookDto> UpdateBookAsync(int id, UpdateBookDto updateBookDto);
        Task<bool> DeleteBookAsync(int id);
        Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);
        Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(int categoryId);
        Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(int authorId);
        Task<IEnumerable<BookDto>> GetBooksByPublisherAsync(int publisherId);
    }
}
```

### **Create Services/BookService.cs:**
```csharp
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;

namespace OnlineBookstore.Services
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BookService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);

            return book != null ? _mapper.Map<BookDto>(book) : null;
        }

        public async Task<BookDetailDto?> GetBookDetailByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews!)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            return book != null ? _mapper.Map<BookDetailDto>(book) : null;
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
        {
            // Validate that related entities exist
            var author = await _context.Authors.FindAsync(createBookDto.AuthorId);
            if (author == null)
                throw new ArgumentException("Author not found");

            var publisher = await _context.Publishers.FindAsync(createBookDto.PublisherId);
            if (publisher == null)
                throw new ArgumentException("Publisher not found");

            var category = await _context.Categories.FindAsync(createBookDto.CategoryId);
            if (category == null)
                throw new ArgumentException("Category not found");

            var book = _mapper.Map<Book>(createBookDto);
            
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Reload with related data for response
            await _context.Entry(book)
                .Reference(b => b.Author)
                .LoadAsync();
            await _context.Entry(book)
                .Reference(b => b.Publisher)
                .LoadAsync();
            await _context.Entry(book)
                .Reference(b => b.Category)
                .LoadAsync();
            await _context.Entry(book)
                .Collection(b => b.Reviews!)
                .LoadAsync();

            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> UpdateBookAsync(int id, UpdateBookDto updateBookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                throw new ArgumentException("Book not found");

            // Validate that related entities exist
            var author = await _context.Authors.FindAsync(updateBookDto.AuthorId);
            if (author == null)
                throw new ArgumentException("Author not found");

            var publisher = await _context.Publishers.FindAsync(updateBookDto.PublisherId);
            if (publisher == null)
                throw new ArgumentException("Publisher not found");

            var category = await _context.Categories.FindAsync(updateBookDto.CategoryId);
            if (category == null)
                throw new ArgumentException("Category not found");

            _mapper.Map(updateBookDto, book);
            
            await _context.SaveChangesAsync();

            return _mapper.Map<BookDto>(book);
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return false;
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllBooksAsync();
            }

            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           b.Author.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           b.Category.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           b.Publisher.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(int categoryId)
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .Where(b => b.CategoryId == categoryId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(int authorId)
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .Where(b => b.AuthorId == authorId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByPublisherAsync(int publisherId)
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .Where(b => b.PublisherId == publisherId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
    }
}
```

---

## 🎮 Step 3: Create Book Controller

### **Understanding Interface vs Implementation**

Before we create the controller, let's understand why we use `IBookService` instead of `BookService` directly:

#### **🔍 Interface (Contract)**
```csharp
// IBookService.cs - Just defines WHAT methods exist (empty contract)
public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllBooksAsync();
    Task<BookDto?> GetBookByIdAsync(int id);
    Task<BookDetailDto?> GetBookDetailByIdAsync(int id);
    // ... other method signatures (no implementation)
}
```

#### **🏗️ Implementation (Real Code)**
```csharp
// BookService.cs - Contains the REAL implementation
public class BookService : IBookService  // ← Implements the interface
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BookService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // REAL implementation of the interface method
    public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
    {
        var books = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .Include(b => b.Category)
            .Include(b => b.Reviews)
            .ToListAsync();

        return _mapper.Map<IEnumerable<BookDto>>(books);
    }
}
```

#### **🔗 How They Connect**
```csharp
// Program.cs - Tells DI container: "When someone asks for IBookService, give them BookService"
builder.Services.AddScoped<IBookService, BookService>();
//     ↑ Interface    ↑ Concrete Implementation
```

#### **🎯 Why This Pattern?**
- **Controller doesn't know** about `BookService` implementation details
- **Easy to swap** implementations (e.g., for testing or caching)
- **Follows SOLID principles** (Dependency Inversion)
- **Better testability** - can mock the interface easily

### **Create Controllers/BooksController.cs:**
```csharp
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving books", error = ex.Message });
            }
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                
                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the book", error = ex.Message });
            }
        }

        // GET: api/books/5/detail
        [HttpGet("{id}/detail")]
        public async Task<ActionResult<BookDetailDto>> GetBookDetail(int id)
        {
            try
            {
                var book = await _bookService.GetBookDetailByIdAsync(id);
                
                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the book details", error = ex.Message });
            }
        }

        // GET: api/books/search?q=harry
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDto>>> SearchBooks([FromQuery] string q)
        {
            try
            {
                var books = await _bookService.SearchBooksAsync(q);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching books", error = ex.Message });
            }
        }

        // GET: api/books/category/5
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByCategory(int categoryId)
        {
            try
            {
                var books = await _bookService.GetBooksByCategoryAsync(categoryId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving books by category", error = ex.Message });
            }
        }

        // GET: api/books/author/5
        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByAuthor(int authorId)
        {
            try
            {
                var books = await _bookService.GetBooksByAuthorAsync(authorId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving books by author", error = ex.Message });
            }
        }

        // GET: api/books/publisher/5
        [HttpGet("publisher/{publisherId}")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByPublisher(int publisherId)
        {
            try
            {
                var books = await _bookService.GetBooksByPublisherAsync(publisherId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving books by publisher", error = ex.Message });
            }
        }

        // POST: api/books
        [HttpPost]
        public async Task<IActionResult> CreateBook(CreateBookDto createBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var book = await _bookService.CreateBookAsync(createBookDto);
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the book", error = ex.Message });
            }
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDto updateBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _bookService.UpdateBookAsync(id, updateBookDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the book", error = ex.Message });
            }
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var result = await _bookService.DeleteBookAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = "Book not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the book", error = ex.Message });
            }
        }
    }
}
```

#### **🔄 What Actually Happens When Controller Uses Interface**

```csharp
// 1. Controller asks for IBookService
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService; // ← Interface type

    public BooksController(IBookService bookService)
    {
        _bookService = bookService; // ← Gets REAL BookService instance
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
    {
        // 2. This calls the REAL implementation in BookService
        var books = await _bookService.GetAllBooksAsync();
        return Ok(books);
    }
}
```

**The Process:**
1. **Controller asks for** `IBookService` (interface)
2. **DI Container looks up** the mapping: `IBookService` → `BookService`
3. **DI Container creates** a new `BookService` instance
4. **DI Container injects** the `BookService` instance into the controller
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

            // Book mappings
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 
                    src.Reviews != null && src.Reviews.Any() 
                        ? src.Reviews.Average(r => r.Rating) 
                        : 0.0));

            CreateMap<Book, BookDetailDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 
                    src.Reviews != null && src.Reviews.Any() 
                        ? src.Reviews.Average(r => r.Rating) 
                        : 0.0));

            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();
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
builder.Services.AddScoped<IBookService, BookService>(); // Add this line
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPayPalService, PayPalService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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
app.MapGet("/", () => "Online Bookstore API is running! Visit /swagger for API documentation.");

// Seed the database
await DbInitializer.SeedAsync(app);

app.Run();
```

---

## 🧪 Step 6: Test Your Book Management

1. **Start the backend application:**
   ```bash
   # Backend
   dotnet run
   ```

2. **Test API endpoints using Postman or curl:**
   ```bash
# Get all books
GET https://localhost:7273/api/books

# Get specific book
GET https://localhost:7273/api/books/1

# Get book with detailed reviews
GET https://localhost:7273/api/books/1/detail

# Search books
GET https://localhost:7273/api/books/search?q=harry

# Get books by category
GET https://localhost:7273/api/books/category/1

# Get books by author
GET https://localhost:7273/api/books/author/1

# Get books by publisher
GET https://localhost:7273/api/books/publisher/1

# Create book
POST https://localhost:7273/api/books
Content-Type: application/json

{
  "title": "Harry Potter and the Philosopher's Stone",
  "description": "The first book in the Harry Potter series",
  "price": 29.99,
  "stockQuantity": 100,
  "coverImageUrl": "https://example.com/harry-potter.jpg",
  "authorId": 1,
  "publisherId": 1,
  "categoryId": 1
}

# Update book
PUT https://localhost:7273/api/books/1
Content-Type: application/json

{
  "title": "Harry Potter and the Philosopher's Stone (Updated)",
  "description": "Updated description",
  "price": 34.99,
  "stockQuantity": 150,
  "coverImageUrl": "https://example.com/harry-potter-updated.jpg",
  "authorId": 1,
  "publisherId": 1,
  "categoryId": 1
}

# Delete book
DELETE https://localhost:7273/api/books/1
   ```

3. **Test the Swagger UI:**
   - Navigate to `https://localhost:7273/swagger`
   - Test all the book endpoints directly from the browser

---

## 🏆 Best Practices

### **Backend Services:**
- Use **direct DbContext access** for simplicity
- Implement **proper error handling** for complex relationships
- Use **AutoMapper** for object mapping with complex relationships
- Keep **services focused** on business logic
- Return **clean DTOs** from services
- Handle **business logic** in service layer

### **API Design:**
- Use **consistent error handling**
- Implement **proper HTTP status codes**
- Use **validation attributes** on DTOs
- Handle **database constraints** gracefully
- Provide **meaningful error messages**

### **Book-Specific Considerations:**
- **Relationship validation**: Ensure Author, Publisher, and Category exist
- **Review data handling**: Calculate average ratings and review counts
- **Search functionality**: Search across multiple fields (title, author, category, publisher)
- **Filtering options**: Get books by category, author, or publisher
- **Detailed views**: Separate endpoints for basic and detailed book information

---

## ✅ What You've Accomplished

- ✅ Created backend book service with **complete CRUD operations**
- ✅ **Added search functionality** across multiple fields
- ✅ **Implemented filtering** by category, author, and publisher
- ✅ **Handled complex relationships** with Author, Publisher, and Category
- ✅ **Added review data management** with ratings and counts
- ✅ Implemented proper error handling and validation
- ✅ Set up dependency injection for book service
- ✅ Created clean API endpoints with proper HTTP responses
- ✅ **Implemented simple service architecture** with direct DbContext access
- ✅ **Added detailed book views** with complete review information

---

## 🚀 Next Steps

Your book management backend is now complete! In the next section, we'll implement shopping cart and order management.

**You've successfully created a complete book management backend with complex relationships. Great job!**

---

**Next up:**
- [Section 13: Managing Shopping Cart & Orders](./13-MANAGING-SHOPPING-CART-ORDERS.md) 

## **Immediate Actions to Try:**

### **1. Test These URLs First:**
- `https://onlinebookstore-avdyfqh4byd4fhb2.australiaeast-01.azurewebsites.net/swagger`
- `https://onlinebookstore-avdyfqh4byd4fhb2.australiaeast-01.azurewebsites.net/api/books`

### **2. Add a Default Route:**
Add this to your `Program.cs` after `app.MapControllers();`:

```csharp
app.MapGet("/", () => "Online Bookstore API is running! Visit /swagger for API documentation.");
```

### **3. Check Application Logs:**
- Go to Azure Portal → Your Web App
- Click "Log stream" to see if there are startup errors

### **4. Verify Configuration:**
- Check if your connection string is set in Azure Configuration
- Ensure `ASPNETCORE_ENVIRONMENT` is set to `Production`

The `/swagger` URL should definitely work and show your API documentation. If that doesn't work, there's likely a startup error that needs to be fixed first.

Try the Swagger URL first and let me know what you see! 