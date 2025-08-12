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

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// REPLACE the existing Authentication configuration with JWT Bearer authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");                    // Get JWT settings from appsettings.json
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHere");  // Convert secret key to bytes

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
         IssuerSigningKey = new SymmetricSecurityKey(key),                          
         ValidateIssuer = true,                                                     
         ValidateAudience = true,                                                   
         ValidIssuer = jwtSettings["Issuer"],                                      
         ValidAudience = jwtSettings["Audience"],                                  
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
        policy.WithOrigins("https://localhost:52441", "http://localhost:52441")
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
app.UseCors("AllowFrontend"); // Add CORS middleware
app.UseSession(); // Add session middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "Online Bookstore API is running! Visit /swagger for API documentation.");

// Seed the database
DbInitializer.SeedAsync(app).Wait();

app.Run();