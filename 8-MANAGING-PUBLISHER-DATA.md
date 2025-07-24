# Section 8: Managing Publisher Data

Welcome to the publisher management phase! In this section, we'll create the backend services for publisher operations using simple services with direct DbContext access. This will serve as a template for managing other entities like categories and books.

---

## 🎯 What You'll Learn

- How to create backend services for publisher operations
- How to implement CRUD operations (Create, Read, Update, Delete)
- How to handle validation and error states
- How to structure services for reusability
- How to use direct DbContext access in services

---

## 🏗️ Step 1: Create Publisher DTOs

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

---

## 🎮 Step 2: Create Publisher Service

### **Create Services/IPublisherService.cs:**
```csharp
using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface IPublisherService
    {
        Task<IEnumerable<PublisherDto>> GetAllPublishersAsync();
        Task<PublisherDto?> GetPublisherByIdAsync(int id);
        Task<PublisherDto> CreatePublisherAsync(CreatePublisherDto createPublisherDto);
        Task<PublisherDto> UpdatePublisherAsync(int id, UpdatePublisherDto updatePublisherDto);
        Task<bool> DeletePublisherAsync(int id);
        Task<IEnumerable<PublisherDto>> SearchPublishersAsync(string searchTerm);
    }
}
```

### **Create Services/PublisherService.cs:**
```csharp
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;

namespace OnlineBookstore.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PublisherService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PublisherDto>> GetAllPublishersAsync()
        {
            var publishers = await _context.Publishers
                .Include(p => p.Books)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PublisherDto>>(publishers);
        }

        public async Task<PublisherDto?> GetPublisherByIdAsync(int id)
        {
            var publisher = await _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefaultAsync(p => p.Id == id);

            return publisher != null ? _mapper.Map<PublisherDto>(publisher) : null;
        }

        public async Task<PublisherDto> CreatePublisherAsync(CreatePublisherDto createPublisherDto)
        {
            var publisher = _mapper.Map<Publisher>(createPublisherDto);
            
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            // For new publishers, BookCount will be 0 (no books yet)
            var publisherDto = _mapper.Map<PublisherDto>(publisher);
            publisherDto.BookCount = 0; // Explicitly set for new publishers

            return publisherDto;
        }

        public async Task<PublisherDto> UpdatePublisherAsync(int id, UpdatePublisherDto updatePublisherDto)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
                throw new ArgumentException("Publisher not found");

            _mapper.Map(updatePublisherDto, publisher);
            
            await _context.SaveChangesAsync();

            return _mapper.Map<PublisherDto>(publisher);
        }

        public async Task<bool> DeletePublisherAsync(int id)
        {
            var publisher = await _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (publisher == null)
            {
                return false;
            }

            if (publisher.Books != null && publisher.Books.Any())
            {
                throw new InvalidOperationException("Cannot delete publisher with existing books");
            }

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PublisherDto>> SearchPublishersAsync(string searchTerm)
        {
            // Business logic: Empty search = return all publishers
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllPublishersAsync(); // Returns all publishers
            }

            var publishers = await _context.Publishers
                .Include(p => p.Books)
                .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .ToListAsync();

            return _mapper.Map<IEnumerable<PublisherDto>>(publishers);
        }


    }
}
```

---

## 🎮 Step 3: Create Publisher Controller

### **Create Controllers/PublishersController.cs:**
```csharp
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        // GET: api/publishers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherDto>>> GetPublishers()
        {
            try
            {
                var publishers = await _publisherService.GetAllPublishersAsync();
                return Ok(publishers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving publishers", error = ex.Message });
            }
        }

        // GET: api/publishers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherDto>> GetPublisher(int id)
        {
            try
            {
                var publisher = await _publisherService.GetPublisherByIdAsync(id);
                
                if (publisher == null)
                {
                    return NotFound(new { message = "Publisher not found" });
                }

                return Ok(publisher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the publisher", error = ex.Message });
            }
        }

        // GET: api/publishers/search?q=searchTerm
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PublisherDto>>> SearchPublishers([FromQuery] string q)
        {
            try
            {
                var publishers = await _publisherService.SearchPublishersAsync(q);
                return Ok(publishers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching publishers", error = ex.Message });
            }
        }



        // POST: api/publishers
        [HttpPost]
        public async Task<IActionResult> CreatePublisher(CreatePublisherDto createPublisherDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var publisher = await _publisherService.CreatePublisherAsync(createPublisherDto);
                return CreatedAtAction(nameof(GetPublisher), new { id = publisher.Id }, publisher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the publisher", error = ex.Message });
            }
        }

        // PUT: api/publishers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublisher(int id, UpdatePublisherDto updatePublisherDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _publisherService.UpdatePublisherAsync(id, updatePublisherDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the publisher", error = ex.Message });
            }
        }

        // DELETE: api/publishers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            try
            {
                var result = await _publisherService.DeletePublisherAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = "Publisher not found" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the publisher", error = ex.Message });
            }
        }
    }
}
```

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

            // Publisher mappings
            CreateMap<Publisher, PublisherDto>()
                .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books != null ? src.Books.Count : 0));

            CreateMap<CreatePublisherDto, Publisher>();
            CreateMap<UpdatePublisherDto, Publisher>();
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
builder.Services.AddScoped<IPublisherService, PublisherService>(); // Add this line

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

## 🧪 Step 6: Test Your Publisher Management

1. **Start the backend application:**
   ```bash
   # Backend
   dotnet run
   ```

2. **Test API endpoints using Postman or curl:**
   ```bash
# Get all publishers
GET https://localhost:7273/api/publishers

# Get specific publisher
GET https://localhost:7273/api/publishers/1

# Create publisher
POST https://localhost:7273/api/publishers
Content-Type: application/json

{
  "name": "Penguin Random House",
  "description": "One of the world's leading publishers"
}

# Update publisher
PUT https://localhost:7273/api/publishers/1
Content-Type: application/json

{
  "name": "Penguin Random House Updated",
  "description": "Updated description for Penguin Random House"
}

# Delete publisher
DELETE https://localhost:7273/api/publishers/1

# Search publishers
GET https://localhost:7273/api/publishers/search?q=Penguin


   ```

3. **Test the Swagger UI:**
   - Navigate to `https://localhost:7273/swagger`
   - Test all the publisher endpoints directly from the browser

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

---

## ✅ What You've Accomplished

- ✅ Created backend publisher service with **complete CRUD operations**
- ✅ **Added search functionality** for finding publishers by name/description

- ✅ Implemented proper error handling and validation
- ✅ Set up dependency injection for publisher service
- ✅ Created clean API endpoints with proper HTTP responses
- ✅ **Implemented simple service architecture** with direct DbContext access

---

## 🚀 Next Steps

Your publisher management backend is now complete! In the next section, we'll implement category management using the same simple approach.

**You've successfully created a complete publisher management backend. Great job!**

---

**Next up:**
- [Section 10: Managing Category Data](./11-MANAGING-CATEGORY-DATA.md)

