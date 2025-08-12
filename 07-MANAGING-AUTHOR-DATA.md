# Section 7: Managing Author Data and Services

## 1. Introduction
Welcome to the author management phase! In this section, you'll implement backend services and controllers for author operations. This will serve as a template for managing other entities like publishers and categories.

---

## 2. What You'll Learn
- How to create backend services for author operations
- How to implement CRUD operations (Create, Read, Update, Delete)
- How to handle validation and error states
- How to structure services for reusability

---

## 3. Implementing the Author Service

### Step 1: Create Services/IAuthorService.cs
```csharp
using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync();
        Task<AuthorDto?> GetAuthorByIdAsync(int id);
        Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto);
        Task<AuthorDto> UpdateAuthorAsync(int id, UpdateAuthorDto updateAuthorDto);
        Task<bool> DeleteAuthorAsync(int id);
        Task<IEnumerable<AuthorDto>> SearchAuthorsAsync(string searchTerm);
    }
}
```

### Step 2: Create Services/AuthorService.cs
```csharp
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;

namespace OnlineBookstore.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AuthorService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
        {
            var authors = await _context.Authors
                .Include(a => a.Books)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AuthorDto>>(authors);
        }

        public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            return author != null ? _mapper.Map<AuthorDto>(author) : null;
        }

        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
        {
            var author = _mapper.Map<Author>(createAuthorDto);
            
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            // For new authors, BookCount will be 0 (no books yet)
            var authorDto = _mapper.Map<AuthorDto>(author);
            authorDto.BookCount = 0; // Explicitly set for new authors

            return authorDto;
        }

        public async Task<AuthorDto> UpdateAuthorAsync(int id, UpdateAuthorDto updateAuthorDto)
        {
          // The ?? is the null-coalescing operator in C#. It's a shorthand way to handle null values.
            var author = await _context.Authors.FindAsync(id) ?? throw new ArgumentException("Author not found");

            _mapper.Map(updateAuthorDto, author);
            await _context.SaveChangesAsync();

            return _mapper.Map<AuthorDto>(author);
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return false;
            }

            if (author.Books != null && author.Books.Any())
            {
                throw new InvalidOperationException("Cannot delete author with existing books");
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<AuthorDto>> SearchAuthorsAsync(string searchTerm)
        {
            // Business logic: Empty search = return all authors
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAuthorsAsync(); // Returns all authors
            }

            var authors = await _context.Authors
                .Include(a => a.Books)
                .Where(a => a.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return _mapper.Map<IEnumerable<AuthorDto>>(authors);
        }
    }
}
```

---

## 4. Implementing the Authors Controller

### Step 1: Create Controllers/AuthorsController.cs
```csharp
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        // GET: api/authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            try
            {
                var authors = await _authorService.GetAllAuthorsAsync();
                return Ok(authors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving authors", error = ex.Message });
            }
        }

        // GET: api/authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
        {
            try
            {
                var author = await _authorService.GetAuthorByIdAsync(id);
                
                if (author == null)
                {
                    return NotFound(new { message = "Author not found" });
                }

                return Ok(author);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the author", error = ex.Message });
            }
        }

        // GET: api/authors/search?q=searchTerm
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> SearchAuthors([FromQuery] string q)
        {
            try
            {
                var authors = await _authorService.SearchAuthorsAsync(q);
                return Ok(authors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching authors", error = ex.Message });
            }
        }

        // POST: api/authors
        [HttpPost]
        public async Task<IActionResult> CreateAuthor(CreateAuthorDto createAuthorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = await _authorService.CreateAuthorAsync(createAuthorDto);
                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the author", error = ex.Message });
            }
        }

        // PUT: api/authors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, UpdateAuthorDto updateAuthorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _authorService.UpdateAuthorAsync(id, updateAuthorDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the author", error = ex.Message });
            }
        }

        // DELETE: api/authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var result = await _authorService.DeleteAuthorAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = "Author not found" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the author", error = ex.Message });
            }
        }
    }
}
```

---

## 5. Registering Services in Program.cs
Add the following to your Program.cs to register the service:
```csharp
// Register all services
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPayPalService, PayPalService>();
builder.Services.AddScoped<IAuthService, AuthService>();
```

---

## 6. Testing the Endpoints
- Use Postman, curl, or Swagger to test the API endpoints for authors.
- Example requests:
  - GET /api/authors
  - POST /api/authors
  - PUT /api/authors/{id}
  - DELETE /api/authors/{id}

---

## 7. Best Practices Recap
- Use dependency injection for services
- Implement proper error handling
- Use AutoMapper for object mapping
- Keep services focused on business logic
- Keep controllers focused on HTTP concerns

---

## 8. Next Steps
- Implement similar services and controllers for publishers, categories, and other entities.
- Continue to the next section for more advanced features! 