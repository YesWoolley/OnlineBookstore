# Section 15: ASP.NET Core Identity Framework

Welcome to the authentication and authorization phase! In this section, we'll implement user authentication using ASP.NET Core Identity with JWT tokens. This will provide the security foundation for all other features in your e-bookstore.

---

## 🎯 What You'll Learn

- How to set up ASP.NET Core Identity with Entity Framework
- How to implement JWT token authentication
- How to create user registration and login endpoints
- How to secure API endpoints with authorization
- How to handle user-specific data (cart, orders, reviews)
- How to implement role-based access control

---

## 🏗️ Step 1: Configure Identity in Program.cs

### **Update Program.cs with detailed comments:**

```csharp
// Add these new using statements for JWT authentication
using Microsoft.AspNetCore.Authentication.JwtBearer;  // For JWT Bearer authentication
using Microsoft.IdentityModel.Tokens;                 // For JWT token validation
using System.Text;                                   // For encoding JWT secret key

// Existing using statements
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using OnlineBookstore.Services;

var builder = WebApplication.CreateBuilder(args);

// Change from MVC controllers to API controllers
builder.Services.AddControllers();                           // Use API controllers instead of MVC controllers

// Configure DbContext (existing code)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// REPLACE the existing Identity configuration with enhanced security settings
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password security requirements
    options.Password.RequireDigit = true;                    // Must contain at least one number
    options.Password.RequireLowercase = true;                // Must contain at least one lowercase letter
    options.Password.RequireUppercase = true;                // Must contain at least one uppercase letter
    options.Password.RequireNonAlphanumeric = true;          // Must contain at least one special character
    options.Password.RequiredLength = 8;                     // Minimum password length

    // User account settings
    options.User.RequireUniqueEmail = true;                  // Each email must be unique
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";  // Allowed username characters

    // Account lockout settings for security
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);    // Lock account for 5 minutes after failed attempts
    options.Lockout.MaxFailedAccessAttempts = 5;                         // Maximum failed login attempts before lockout
    options.Lockout.AllowedForNewUsers = true;                           // Enable lockout for new users
})
.AddEntityFrameworkStores<AppDbContext>()                    // Use Entity Framework for user storage
.AddDefaultTokenProviders();                                 // Add default token providers for password reset, etc.

// REPLACE the existing Authentication configuration with JWT Bearer authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");                    // Get JWT settings from appsettings.json
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHere");  // Convert secret key to bytes

// Configure authentication to use JWT Bearer tokens
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;      // Use JWT Bearer for authentication
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;         // Use JWT Bearer for challenges
})
.AddJwtBearer(options =>                                                             // Add JWT Bearer authentication
{
    options.RequireHttpsMetadata = false;                                           // Don't require HTTPS metadata (for development)
    options.SaveToken = true;                                                       // Save the token for later use
    options.TokenValidationParameters = new TokenValidationParameters                 // Configure token validation rules
    {
        ValidateIssuerSigningKey = true,                                            // Validate the signing key
        IssuerSigningKey = new SymmetricSecurityKey(key),                          // Set the signing key for token validation
        ValidateIssuer = true,                                                      // Validate the token issuer
        ValidateAudience = true,                                                    // Validate the token audience
        ValidIssuer = jwtSettings["Issuer"],                                       // Set the valid issuer from config
        ValidAudience = jwtSettings["Audience"],                                   // Set the valid audience from config
        ClockSkew = TimeSpan.Zero                                                  // No clock skew tolerance
    };
});

// ADD new Authorization policies for role-based access control
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));           // Policy requiring Admin role
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin"));    // Policy requiring User or Admin role
});

// Configure AutoMapper (existing code)
builder.Services.AddAutoMapper(typeof(Program));

// Register Services (existing services)
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// ADD new AuthService registration for dependency injection
builder.Services.AddScoped<IAuthService, AuthService>();    // Register authentication service

// Configure Swagger/OpenAPI (existing code)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline (existing code)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// REMOVE these lines (they're for MVC, not API):
// app.UseStaticFiles();                                   // Remove - not needed for API
// app.UseSession();                                       // Remove - not needed for API
// app.MapControllerRoute(...);                            // Remove - this is MVC routing

// UPDATE the middleware pipeline for API
app.UseHttpsRedirection();                                // Redirect HTTP to HTTPS

// Add Authentication & Authorization middleware in correct order
app.UseAuthentication();                                   // Enable authentication middleware
app.UseAuthorization();                                    // Enable authorization middleware

app.MapControllers();                                      // Map API controllers (not MVC routes)

// Seed the database (existing code)
await DbInitializer.SeedAsync(app);

app.Run();
```

---

## 🏗️ Step 2: Update appsettings.json

### **Update appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OnlineBookstore;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyHereMakeItLongAndComplex",// This is a placeholder value that should be replaced with a real, secure key.
    "Issuer": "OnlineBookstore",
    "Audience": "OnlineBookstoreUsers",
    "ExpirationHours": 24
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## 🏗️ Step 3: Create Authentication DTOs

### **Create DTOs/UserDto.cs:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends user data TO clients after authentication
    public class UserDto
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<string> Roles { get; set; } = new List<string>();
    }

    // Input DTO: Receives registration data FROM clients
    public class RegisterDto
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }

    // Input DTO: Receives login data FROM clients
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

    // Response DTO: Sends authentication result TO clients
    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public UserDto? User { get; set; }
        public string? Message { get; set; }
    }

    // Input DTO: Receives profile update data FROM clients
    public class UpdateProfileDto
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }

    // Input DTO: Receives password change data FROM clients
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; } = null!;

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
```

---

## 🎮 Step 4: Create Authentication Service

### **Create Services/IAuthService.cs:**
```csharp
using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<UserDto?> GetUserProfileAsync(string userId);
        Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
```

### **Create Services/AuthService.cs:**
```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;

namespace OnlineBookstore.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "User with this email already exists"
                };
            }

            existingUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUser != null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Username is already taken"
                };
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");

            // Generate tokens
            var token = await GenerateJwtTokenAsync(user);
            var refreshToken = await GenerateRefreshTokenAsync(user);

            return new AuthResultDto
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                User = await GetUserDtoAsync(user)
            };
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Generate tokens
            var token = await GenerateJwtTokenAsync(user);
            var refreshToken = await GenerateRefreshTokenAsync(user);

            return new AuthResultDto
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                User = await GetUserDtoAsync(user)
            };
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Invalid refresh token"
                };
            }

            // Generate new tokens
            var newToken = await GenerateJwtTokenAsync(user);
            var newRefreshToken = await GenerateRefreshTokenAsync(user);

            return new AuthResultDto
            {
                Success = true,
                Token = newToken,
                RefreshToken = newRefreshToken,
                User = await GetUserDtoAsync(user)
            };
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }
            return true;
        }

        public async Task<UserDto?> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null ? await GetUserDtoAsync(user) : null;
        }

        public async Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            user.FirstName = updateProfileDto.FirstName ?? user.FirstName;
            user.LastName = updateProfileDto.LastName ?? user.LastName;
            user.PhoneNumber = updateProfileDto.PhoneNumber ?? user.PhoneNumber;

            await _userManager.UpdateAsync(user);
            return await GetUserDtoAsync(user);
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            return result.Succeeded;
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // TODO: Send email with reset token
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("FirstName", user.FirstName ?? ""),
                new Claim("LastName", user.LastName ?? "")
            };

            // Add roles to claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["JwtSettings:ExpirationHours"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
        {
            var refreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);
            return refreshToken;
        }

        private async Task<UserDto> GetUserDtoAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = roles.ToList();
            return userDto;
        }
    }
}
```

---

## 🎮 Step 5: Create Authentication Controller

### **Create Controllers/AuthController.cs:**
```csharp
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;
using System.Security.Claims;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResultDto>> Register(RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RegisterAsync(registerDto);
                
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDto>> Login(LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.LoginAsync(loginDto);
                
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        // POST: api/auth/refresh
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResultDto>> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshToken);
                
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while refreshing token", error = ex.Message });
            }
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            try
            {
                await _authService.LogoutAsync(refreshToken);
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during logout", error = ex.Message });
            }
        }

        // GET: api/auth/profile
        [HttpGet("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _authService.GetUserProfileAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving profile", error = ex.Message });
            }
        }

        // PUT: api/auth/profile
        [HttpPut("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<UserDto>> UpdateProfile(UpdateProfileDto updateProfileDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _authService.UpdateProfileAsync(userId, updateProfileDto);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating profile", error = ex.Message });
            }
        }

        // POST: api/auth/change-password
        [HttpPost("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
                if (!result)
                {
                    return BadRequest(new { message = "Failed to change password" });
                }

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while changing password", error = ex.Message });
            }
        }
    }
}
```

---

## 🎮 Step 6: Create User Controller

### **Create Controllers/UsersController.cs:**
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;
using System.Security.Claims;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IShoppingCartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IReviewService _reviewService;

        public UsersController(
            IShoppingCartService cartService,
            IOrderService orderService,
            IReviewService reviewService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _reviewService = reviewService;
        }

        // GET: api/users/cart
        [HttpGet("cart")]
        public async Task<ActionResult<IEnumerable<ShoppingCartItemDto>>> GetUserCart()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var cartItems = await _cartService.GetUserCartAsync(userId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving cart", error = ex.Message });
            }
        }

        // GET: api/users/orders
        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var orders = await _orderService.GetUserOrdersAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving orders", error = ex.Message });
            }
        }

        // GET: api/users/reviews
        [HttpGet("reviews")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetUserReviews()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var reviews = await _reviewService.GetReviewsByUserAsync(userId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving reviews", error = ex.Message });
            }
        }
    }
}
```

---

## ⚙️ Step 7: Update AutoMapper Profile

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

            // User mappings
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<RegisterDto, ApplicationUser>();
            CreateMap<UpdateProfileDto, ApplicationUser>();
        }
    }
}
```

---

## ⚙️ Step 8: Update ApplicationUser Model

### **Update Models/ApplicationUser.cs:**
```csharp
using EbooksPlatform.Models;
using Microsoft.AspNetCore.Identity;

namespace OnlineBookstore.Models
{
    public class ApplicationUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        // Navigation properties for relationships
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; }
    }
}
```

---

## 🧪 Step 9: Test Your Authentication

1. **Start the backend application:**
   ```bash
   # Backend
   dotnet run
   ```

2. **Test API endpoints using Postman or curl:**
   ```bash
# Register new user
POST https://localhost:7273/api/auth/register
Content-Type: application/json

{
  "userName": "john_doe",
  "email": "john@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "firstName": "John",
  "lastName": "Doe"
}

# Login
POST https://localhost:7273/api/auth/login
Content-Type: application/json

{
  "userName": "john_doe",
  "password": "Password123!"
}

# Get user profile (requires Authorization header)
GET https://localhost:7273/api/auth/profile
Authorization: Bearer YOUR_JWT_TOKEN

# Update profile (requires Authorization header)
PUT https://localhost:7273/api/auth/profile
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "firstName": "John Updated",
  "lastName": "Doe Updated",
  "phoneNumber": "+1234567890"
}

# Change password (requires Authorization header)
POST https://localhost:7273/api/auth/change-password
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "currentPassword": "Password123!",
  "newPassword": "NewPassword123!",
  "confirmNewPassword": "NewPassword123!"
}

# Get user cart (requires Authorization header)
GET https://localhost:7273/api/users/cart
Authorization: Bearer YOUR_JWT_TOKEN

# Get user orders (requires Authorization header)
GET https://localhost:7273/api/users/orders
Authorization: Bearer YOUR_JWT_TOKEN

# Get user reviews (requires Authorization header)
GET https://localhost:7273/api/users/reviews
Authorization: Bearer YOUR_JWT_TOKEN

# Refresh token
POST https://localhost:7273/api/auth/refresh
Content-Type: application/json

"YOUR_REFRESH_TOKEN"

# Logout
POST https://localhost:7273/api/auth/logout
Content-Type: application/json

"YOUR_REFRESH_TOKEN"
   ```

3. **Test the Swagger UI:**
   - Navigate to `https://localhost:7273/swagger`
   - Test all the authentication endpoints directly from the browser

---

## 🏆 Best Practices

### **Authentication Best Practices:**
- **Strong password requirements** (uppercase, lowercase, numbers, symbols)
- **JWT token expiration** (24 hours for access tokens)
- **Refresh token rotation** for security
- **Email confirmation** for new accounts
- **Password reset** functionality

### **Security Best Practices:**
- **HTTPS only** in production
- **Token validation** on all protected endpoints
- **Role-based authorization** for different user types
- **Input validation** for all user inputs
- **Rate limiting** for login attempts

### **User Experience Best Practices:**
- **Clear error messages** for authentication failures
- **User-friendly validation** messages
- **Automatic token refresh** on the client
- **Remember me** functionality
- **Profile management** with avatar support

---

## ✅ What You've Accomplished

- ✅ **Set up ASP.NET Core Identity** with Entity Framework
- ✅ **Implemented JWT authentication** with refresh tokens
- ✅ **Created user registration and login** endpoints
- ✅ **Added user profile management** functionality
- ✅ **Secured API endpoints** with authorization
- ✅ **Implemented user-specific data** (cart, orders, reviews)
- ✅ **Added password change** functionality
- ✅ **Created comprehensive authentication** system

---

## 🚀 Next Steps

Your authentication system is now complete! In the next section, we'll implement payment integration.

**You've successfully created a secure authentication foundation for your e-bookstore. Great job!**

---

**Next up:**
- [Section 16: Integrating Payments](./16-INTEGRATING-PAYMENTS.md) 