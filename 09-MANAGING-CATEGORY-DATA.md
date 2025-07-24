# Section 9: Managing Category Data

Welcome to the category management phase! In this section, we'll create the backend services for category operations using simple services with direct DbContext access. This follows the same pattern we established for publishers and will serve as a template for managing other entities.

---

## 🎯 What You'll Learn

- How to create backend services for category operations
- How to implement CRUD operations (Create, Read, Update, Delete)
- How to handle validation and error states
- How to structure services for reusability
- How to use direct DbContext access in services
- How to implement search functionality for categories

---

## 🏗️ Step 1: Create Category DTOs

### **Create DTOs/CategoryDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends category data TO clients with book count for display
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int BookCount { get; set; }
    }

    // Input DTO: Receives category data FROM clients for creation (validation prevents empty names)
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
    }

    // Input DTO: Receives category data FROM clients for updates (validation maintains data quality)
    public class UpdateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
    }
}
```

---

## 🎮 Step 2: Create Category Service

### **Create Services/ICategoryService.cs:**
```csharp
using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<CategoryDto>> SearchCategoriesAsync(string searchTerm);
    }
}
```

### **Create Services/CategoryService.cs:**
```csharp
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;

namespace OnlineBookstore.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoryService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Books)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Books)
                .FirstOrDefaultAsync(c => c.Id == id);

            return category != null ? _mapper.Map<CategoryDto>(category) : null;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);
            
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // For new categories, BookCount will be 0 (no books yet)
            var categoryDto = _mapper.Map<CategoryDto>(category);
            categoryDto.BookCount = 0; // Explicitly set for new categories

            return categoryDto;
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new ArgumentException("Category not found");

            _mapper.Map(updateCategoryDto, category);
            
            await _context.SaveChangesAsync();

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Books)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return false;
            }

            if (category.Books != null && category.Books.Any())
            {
                throw new InvalidOperationException("Cannot delete category with existing books");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CategoryDto>> SearchCategoriesAsync(string searchTerm)
        {
            // Business logic: Empty search = return all categories
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllCategoriesAsync(); // Returns all categories
            }

            var categories = await _context.Categories
                .Include(c => c.Books)
                .Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
    }
}
```

---

## 🎮 Step 3: Create Category Controller

### **Understanding Interface vs Implementation**

Before we create the controller, let's understand why we use `ICategoryService` instead of `CategoryService` directly:

#### **🔍 Interface (Contract)**
```csharp
// ICategoryService.cs - Just defines WHAT methods exist (empty contract)
public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    // ... other method signatures (no implementation)
}
```

#### **🏗️ Implementation (Real Code)**
```csharp
// CategoryService.cs - Contains the REAL implementation
public class CategoryService : ICategoryService  // ← Implements the interface
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // REAL implementation of the interface method
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _context.Categories
            .Include(c => c.Books)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }
}
```

#### **🔗 How They Connect**
```csharp
// Program.cs - Tells DI container: "When someone asks for ICategoryService, give them CategoryService"
builder.Services.AddScoped<ICategoryService, CategoryService>();
//     ↑ Interface    ↑ Concrete Implementation
```

#### **🎯 Why This Pattern?**
- **Controller doesn't know** about `CategoryService` implementation details
- **Easy to swap** implementations (e.g., for testing or caching)
- **Follows SOLID principles** (Dependency Inversion)
- **Better testability** - can mock the interface easily

### **Create Controllers/CategoriesController.cs:**
```csharp
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving categories", error = ex.Message });
            }
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                
                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the category", error = ex.Message });
            }
        }

        // GET: api/categories/search?q=searchTerm
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> SearchCategories([FromQuery] string q)
        {
            try
            {
                var categories = await _categoryService.SearchCategoriesAsync(q);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching categories", error = ex.Message });
            }
        }

        // POST: api/categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the category", error = ex.Message });
            }
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the category", error = ex.Message });
            }
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = "Category not found" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the category", error = ex.Message });
            }
        }
    }
}
```

#### **🔄 What Actually Happens When Controller Uses Interface**

```csharp
// 1. Controller asks for ICategoryService
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService; // ← Interface type

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService; // ← Gets REAL CategoryService instance
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        // 2. This calls the REAL implementation in CategoryService
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }
}
```

**The Process:**
1. **Controller asks for** `ICategoryService` (interface)
2. **DI Container looks up** the mapping: `ICategoryService` → `CategoryService`
3. **DI Container creates** a new `CategoryService` instance
4. **DI Container injects** the `CategoryService` instance into the controller
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

            // Category mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books != null ? src.Books.Count : 0));

            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
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
builder.Services.AddScoped<ICategoryService, CategoryService>(); // Add this line

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

## 🧪 Step 6: Test Your Category Management

1. **Start the backend application:**
   ```bash
   # Backend
   dotnet run
   ```

2. **Test API endpoints using Postman or curl:**
   ```bash
# Get all categories
GET https://localhost:7273/api/categories

# Get specific category
GET https://localhost:7273/api/categories/1

# Create category
POST https://localhost:7273/api/categories
Content-Type: application/json

{
  "name": "Fiction"
}

# Update category
PUT https://localhost:7273/api/categories/1
Content-Type: application/json

{
  "name": "Science Fiction"
}

# Delete category
DELETE https://localhost:7273/api/categories/1

# Search categories
GET https://localhost:7273/api/categories/search?q=Fiction
   ```

3. **Test the Swagger UI:**
   - Navigate to `https://localhost:7273/swagger`
   - Test all the category endpoints directly from the browser

---

## 🏆 Best Practices

### **Backend Services:**
- Use **direct DbContext access** for simplicity
- Implement **proper error handling**
- Use **AutoMapper** for object mapping
- Keep **services focused** on business logic
- Return **clean DTOs** from services
- Handle **business logic** in service layer

### **API Design:**
- Use **consistent error handling**
- Implement **proper HTTP status codes**
- Use **validation attributes** on DTOs
- Handle **database constraints** gracefully
- Provide **meaningful error messages**

### **Category-Specific Considerations:**
- **Name validation**: Ensure category names are unique and properly formatted
- **Book count tracking**: Display how many books are in each category
- **Deletion constraints**: Prevent deletion of categories with existing books
- **Search functionality**: Allow users to find categories by name

---

## ✅ What You've Accomplished

- ✅ Created backend category service with **complete CRUD operations**
- ✅ **Added search functionality** for finding categories by name
- ✅ Implemented proper error handling and validation
- ✅ Set up dependency injection for category service
- ✅ Created clean API endpoints with proper HTTP responses
- ✅ **Implemented simple service architecture** with direct DbContext access
- ✅ Added **book count tracking** for better user experience

---

## 🚀 Next Steps

Your category management backend is now complete! In the next section, we'll implement book management using the same simple approach.

**You've successfully created a complete category management backend. Great job!**

---

**Next up:**
- [Section 12: Managing Book Data](./12-MANAGING-BOOK-DATA.md) 