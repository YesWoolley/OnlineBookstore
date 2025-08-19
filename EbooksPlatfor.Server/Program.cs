using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;  
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;                
using OnlineBookstore.Data;
using OnlineBookstore.Models;
using OnlineBookstore.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext with environment variable support
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? 
                      builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure Identity
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
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Configure JWT with environment variable support
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? 
                builder.Configuration.GetSection("JwtSettings")["SecretKey"] ?? 
                "Yy5qV1mK8rN2tF4pX9bD6wZ3aL7hE0sR1uG5oC2jM8nT4vQ6yP3kB7zW8xR2dJ9";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? 
                builder.Configuration.GetSection("JwtSettings")["Issuer"] ?? 
                "OnlineBookstore";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? 
                  builder.Configuration.GetSection("JwtSettings")["Audience"] ?? 
                  "OnlineBookstoreUsers";

// Log JWT key information at startup
var keyBytes = Encoding.UTF8.GetBytes(jwtSecret);
Console.WriteLine($"[JWT] Secret key length: {jwtSecret.Length} characters");
Console.WriteLine($"[JWT] Key bytes length: {keyBytes.Length} bytes");
Console.WriteLine($"[JWT] Key bits: {keyBytes.Length * 8} bits");
Console.WriteLine($"[JWT] Secret key preview: {jwtSecret.Substring(0, Math.Min(10, jwtSecret.Length))}...");

// Use plain text key as UTF8 bytes
var key = new SymmetricSecurityKey(keyBytes);

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;     
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;         
})
.AddJwtBearer(options =>                                                             
 {
     options.RequireHttpsMetadata = true;                                           
     options.SaveToken = true;                                                      
     options.TokenValidationParameters = new TokenValidationParameters              
     {
         ValidateIssuerSigningKey = true,                                           
         IssuerSigningKey = key,                                          
         ValidateIssuer = true,                                                     
         ValidateAudience = true,                                                     
                  ValidIssuer = jwtIssuer,      
         ValidAudience = jwtAudience,                                  
         ClockSkew = TimeSpan.Zero                                                
     };
 });

// ADD new Authorization policies for role-based access control
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));           // Policy requiring Admin role
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin"));    // Policy requiring User or Admin role
});

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Register authentication service
builder.Services.AddScoped<IAuthService, AuthService>();

// Register all services
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// Configure PayPal with environment variable support
var paypalClientId = Environment.GetEnvironmentVariable("PAYPAL_CLIENT_ID") ?? 
                     builder.Configuration.GetSection("PayPal")["ClientId"];
var paypalClientSecret = Environment.GetEnvironmentVariable("PAYPAL_CLIENT_SECRET") ?? 
                         builder.Configuration.GetSection("PayPal")["ClientSecret"];
var paypalMode = Environment.GetEnvironmentVariable("PAYPAL_MODE") ?? 
                 builder.Configuration.GetSection("PayPal")["Mode"] ?? "sandbox";

builder.Services.AddScoped<IPayPalService, PayPalService>();    

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Memory Cache and Session
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache(); // Add this for session support
builder.Services.AddSession();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = new[]
        {
            "https://localhost:52441",
            "http://localhost:52441",
            "https://happy-smoke-0d1f54100.2.azurestaticapps.net"
        };
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Force HTTPS binding in development
if (app.Environment.IsDevelopment())
{
    // Ensure HTTPS is available
    app.Urls.Add("https://localhost:7273");
    
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

// Middleware Pipeline 
app.UseHttpsRedirection();

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

app.UseCors("AllowFrontend"); 
app.UseSession(); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoints
app.MapGet("/", () => {
    return Results.Json(new {
        Message = "Online Bookstore API is running! Visit /swagger for API documentation.",
        Status = "Healthy",
        Timestamp = DateTime.UtcNow,
        Environment = app.Environment.EnvironmentName
    });
});
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow, Environment = app.Environment.EnvironmentName });
app.MapGet("/api/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow, Environment = app.Environment.EnvironmentName });

// Seed the database
DbInitializer.SeedAsync(app).Wait();

app.Run();