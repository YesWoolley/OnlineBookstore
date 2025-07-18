# Section 7: Managing Author Data and Services

Welcome to the author management phase! In this section, we'll create the backend services for author operations. This will serve as a template for managing other entities like publishers and categories.

---

## 🔄 **Service vs Controller Return Types**

**Quick Rule:** 
- **Services** return business objects (DTOs, entities)
- **Controllers** return HTTP responses (`IActionResult`)

### **❌ Common Mistake: Using HTTP Methods in Services**

**Never use HTTP response methods in services:**
```csharp
// ❌ WRONG - Service layer
public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
{
    var author = await _context.Authors.FindAsync(id);
    
    if (author == null)
    {
        return NotFound(new { message = "Author not found" }); // ❌ HTTP response in service!
    }
    
    return _mapper.Map<AuthorDto>(author);
}
```

### **✅ Correct Approach: Services Return Data, Controllers Handle HTTP**

```csharp
// ✅ CORRECT - Service layer
public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
{
    var author = await _context.Authors
        .Include(a => a.Books)
        .FirstOrDefaultAsync(a => a.Id == id);

    return author != null ? _mapper.Map<AuthorDto>(author) : null; // ✅ Return data or null
}

// ✅ CORRECT - Controller layer
public class AuthorsController : ControllerBase
{
    public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);
        
        if (author == null)
        {
            return NotFound(new { message = "Author not found" }); // ✅ HTTP response here
        }
        
        return Ok(author);
    }
}
```

### **🎯 Key Principles:**

| Layer | Responsibility | Return Types | Examples |
|-------|----------------|--------------|----------|
| **Service** | Business Logic | DTOs, Entities, null | `AuthorDto`, `IEnumerable<AuthorDto>`, `bool` |
| **Controller** | HTTP Responses | `IActionResult`, `ActionResult<T>` | `Ok()`, `NotFound()`, `BadRequest()` |

---

## 🏗️ **Why Services? Understanding the Controller-Service Relationship**

### **🎭 The Restaurant Analogy**

Think of your application like a **fancy restaurant**:

| Component | Restaurant | Your App | Purpose |
|-----------|------------|----------|---------|
| **Controller** | Waiter | API Endpoint | Takes orders, serves food |
| **Service** | Chef | Business Logic | Prepares the food, knows recipes |
| **Repository** | Kitchen Staff | Data Access | Gets ingredients from storage |
| **Database** | Pantry | SQL Server | Stores all ingredients |

### **🍽️ How It Works in Practice:**

**Without Services (Bad Restaurant):**
```
Customer → Waiter → Waiter cooks food → Waiter serves food
```
*Problem: Waiter is doing everything - taking orders, cooking, serving. Overwhelmed!*

**With Services (Good Restaurant):**
```
Customer → Waiter → Chef → Kitchen Staff → Waiter serves food
```
*Benefit: Each person has a specific job. Efficient and organized!*

### **💻 In Your Code:**

**❌ Without Service Layer (Direct Controller):**
```csharp
// Controllers/AuthorsController.cs (BAD - doing everything)
public class AuthorsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public AuthorsController(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
    {
        // Controller is doing TOO MUCH:
        // 1. Database queries
        // 2. Business logic
        // 3. Data mapping
        // 4. Error handling
        // 5. Validation
        
        var authors = await _context.Authors
            .Include(a => a.Books)
            .ToListAsync();
            
        var authorDtos = authors.Select(a => new AuthorDto
        {
            Id = a.Id,
            Name = a.Name,
            BookCount = a.Books.Count,
            // ... more mapping logic
        });
        
        return Ok(authorDtos);
    }
}
```

**✅ With Service Layer (Clean Architecture):**
```csharp
// Controllers/AuthorsController.cs (GOOD - focused on HTTP)
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;
    
    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }
    
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
    {
        // Controller ONLY handles HTTP concerns:
        // 1. Receives request
        // 2. Calls service
        // 3. Returns response
        
        var authors = await _authorService.GetAllAuthorsAsync();
        return Ok(authors);
    }
}

// Services/AuthorService.cs (Business Logic)
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
        // Service handles business logic:
        // 1. Data access
        // 2. Business rules
        // 3. Data transformation
        // 4. Error handling
        
        var authors = await _context.Authors
            .Include(a => a.Books)
            .ToListAsync();
            
        return _mapper.Map<IEnumerable<AuthorDto>>(authors);
    }
}
```

### **🎯 Benefits of Service Layer:**

| Benefit | Description | Example |
|---------|-------------|---------|
| **Separation of Concerns** | Each class has one job | Controller = HTTP, Service = Business Logic |
| **Reusability** | Service can be used by multiple controllers | AuthorService used by AuthorsController AND AdminController |
| **Testability** | Easy to test business logic separately | Test AuthorService without HTTP concerns |
| **Maintainability** | Changes in business logic don't affect controllers | Update author validation without touching controllers |
| **Scalability** | Add caching, logging, etc. in service layer | Add Redis caching to AuthorService |

### **🔄 The Flow:**

```
API Request
       ↓
   Controller (HTTP Layer)
       ↓
   Service (Business Logic)
       ↓
   Repository (Data Access)
       ↓
   Database
```

### **🧪 Real-World Example:**

**Scenario:** "Get all authors with their book counts"

**Without Service:**
- Controller queries database
- Controller maps data
- Controller handles errors
- Controller validates data
- Controller formats response

**With Service:**
- Controller receives request
- Controller calls `authorService.GetAllAuthorsAsync()`
- Service handles everything else
- Controller returns response

**Result:** Controller is clean, focused, and easy to understand!

---

## 🎯 What You'll Learn

- How to create backend services for author operations
- How to implement CRUD operations (Create, Read, Update, Delete)
- How to handle validation and error states
- How to structure services for reusability

---

## 🏗️ Step 1: Create Backend Author Service

### **Create Services/IAuthorService.cs:**
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

### **Create Services/AuthorService.cs:**
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

## 🎮 Step 2: Update Authors Controller

### **Update Controllers/AuthorsController.cs:**
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

## ⚙️ Step 3: Register Services in Program.cs

**Why Register Services?**
Services need to be registered in the dependency injection container so that ASP.NET Core can automatically create and inject them into controllers. This is like telling the restaurant manager about all the chefs available.

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
builder.Services.AddScoped<IAuthorService, AuthorService>(); //add here

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

## 🧪 Step 4: Test Your Author Management

1. **Start the backend application:**
   ```bash
   # Backend
   dotnet run
   ```

2. **Test API endpoints using Postman or curl:**
   ```bash
# Get all authors
GET https://localhost:7273/api/authors

# Get specific author
GET https://localhost:7273/api/authors/1

# Create author
POST https://localhost:7273/api/authors
Content-Type: application/json

{
  "name": "Jane Doe",
  "biography": "A talented author with many published works."
}

# Update author
PUT https://localhost:7273/api/authors/1
Content-Type: application/json

{
  "name": "Jane Doe Updated",
  "biography": "An updated biography for Jane Doe."
}

# Delete author
DELETE https://localhost:7273/api/authors/1

# Search authors
GET https://localhost:7273/api/authors/search?q=Jane
   ```

3. **Test the Swagger UI:**
   - Navigate to `https://localhost:7273/swagger`
   - Test all the author endpoints directly from the browser

---

## 🏆 Best Practices

### **Backend Services:**
- Use **dependency injection** for services
- Implement **proper error handling**
- Use **AutoMapper** for object mapping
- Keep **services simple** with direct DbContext access
- Return **clean DTOs** from services
- Handle **business logic** in service layer

### **API Design:**
- Use **consistent error handling**
- Implement **proper HTTP status codes**
- Use **validation attributes** on DTOs
- Handle **database constraints** gracefully
- Provide **meaningful error messages**

---

## ✅ What You've Accomplished

- ✅ Created backend author service with **complete CRUD operations**
- ✅ **Added search functionality** for finding authors by name
- ✅ Implemented proper error handling and validation
- ✅ Set up dependency injection for services
- ✅ Created clean API endpoints with proper HTTP responses
- ✅ **Implemented service layer architecture** with separation of concerns

---

## 🚀 Next Steps

Your author management backend is now complete! In the next section, we'll implement publisher management using the same simple approach.

**You've successfully created a complete author management backend. Great job!**

---

**Next up:**
- [Section 8: Managing Publisher Data](./10-MANAGING-PUBLISHER-DATA.md) 