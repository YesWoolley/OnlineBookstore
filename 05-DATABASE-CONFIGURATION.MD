# Section 5: Database Configuration and Management

Welcome to the database setup phase! In this section, we'll configure Entity Framework Core, set up our database context, define relationships using Fluent API, create migrations, and seed our database with initial data.

---

## 🎯 What You'll Learn

- How to configure Entity Framework Core
- How to set up database relationships using Fluent API
- How to create and apply database migrations
- How to seed initial data
- Best practices for database management

---

## 📦 Step 1: Install Required NuGet Packages

First, let's install the necessary Entity Framework Core packages. In your `OnlineBookstore` project, add these packages:

### **Via Package Manager Console:**
```powershell
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design
Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

### **Via .NET CLI:**
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

---

## 🏗️ Step 2: Understanding Why We Need the Data Folder and AppDbContext

### **🎯 The Problem: "We Built Models, But How Do We Use Them?"**

Think of it this way: **We've created beautiful blueprints (our models), but now we need the actual building!**

In Section 4, we built our models like this:
```csharp
public class Book { }
public class Author { }
public class Category { }
// ... etc
```

**But here's the problem:** These are just **C# classes** sitting in memory. They're like beautiful architectural drawings, but there's no actual building yet!

### **🔧 What AppDbContext Actually Does**

The `AppDbContext` is like a **smart translator** that speaks both languages:
- **C# language** (our models)
- **Database language** (SQL)

**Without AppDbContext:** Our models are just floating in space with no connection to reality!

### **🚀 The Magic Happens Here**

**AppDbContext is Like a Magic Wand That:**
1. **"Hey database, create a table for Book model!"**
2. **"Hey database, create a table for Author model!"**
3. **"Hey database, connect Book to Author with foreign keys!"**
4. **"Hey database, when I ask for books, give me C# objects!"**

### **🎯 Real-World Analogy**

**Think of it like a restaurant:**

| Component | Restaurant | Our App |
|-----------|------------|---------|
| **Menu** | Dishes available | Models (Book, Author, etc.) |
| **Kitchen** | Where food is made | Data folder |
| **Chef** | Prepares the food | AppDbContext |
| **Waiters** | Serve the food | Controllers (coming next) |

**Without the kitchen and chef:** You have a menu, but no way to actually get food!

### **🏗️ The `Data` Folder: Our "Construction Site"**

Think of your project like a **well-organized house:**

```
OnlineBookstore/
├── Models/          # 📋 Blueprints (what we built in Section 4)
├── Data/            # 🏗️ Construction site (where we build the database)
├── Controllers/     # 🎮 Control room (coming next)
└── Services/        # ⚙️ Machinery (coming later)
```

**The `Data` folder is where we:**
- **Build the database** (AppDbContext)
- **Manage data access** (repositories)
- **Seed initial data** (DbInitializer)

### **🎯 The "Aha!" Moment**

**Before AppDbContext:**
```csharp
// We have models, but they're useless!
public class Book { }  // Just sitting there, doing nothing
```

**After AppDbContext:**
```csharp
// Now our models come to life!
var allBooks = context.Books.ToList();           // Get all books
var harryPotter = context.Books.Find(1);        // Find specific book
context.Books.Add(newBook);                      // Add new book
context.SaveChanges();                           // Save to database
```

**Without this step:** Our beautiful models would be like having a Ferrari without an engine - they look great but can't actually do anything!

---

## 💡 Step 3: Tips for Database Constraints & Validation

### Tip 1: Validation and Relationships

- **Validate on both sides:** Always validate user input in your DTOs using attributes like [Required], [StringLength], and [Range] to catch errors before they reach your database, and enforce critical rules at the database level using Fluent API or data annotations in your model classes to protect data integrity.
- **Match rules:** Use matching rules in both places (DTO and model) for important fields, so you catch errors early and prevent bad data from being saved.
- **Set sensible defaults:** Use defaults (like .HasDefaultValue(0)) to avoid null or unexpected values in your database.
- **Choose the right tool:** Prefer Fluent API for complex configurations (like decimal precision or relationships) and use data annotations for simple, common constraints.
- **User experience & safety:** API validation improves user experience; database constraints protect your data.
- **Relationship tip:** In Entity Framework, configuring one side of a one-to-many relationship (usually the many-to-one side) is enough if you have navigation properties on both entities.

**Always apply validation on both sides:**

```csharp
// DTO (user input)
[Required, StringLength(200)]
public string Title { get; set; }

// Model/Fluent API (database)
modelBuilder.Entity<Book>()
    .Property(b => b.Title)
    .HasMaxLength(200)
    .IsRequired();
```

### Tip 2: One-Sided Relationship Configuration

If you configure the relationship from one side (usually the "many" side), and you have navigation properties on both entities, Entity Framework will handle the other side automatically. You do NOT need to configure both sides separately.

### Tip 3: Using AutoMapper to Map Entities to DTOs

When you see code like:

```csharp
_mapper.Map<IEnumerable<AuthorDto>>(authors)
```

this is using **AutoMapper**, a library that helps you automatically convert ("map") one type of object to another.

**What does `Map` do?**
- `Map` takes an object (or collection of objects) of one type and creates a new object (or collection) of another type, copying over the relevant data.
- In this case, it converts a list of `Author` entities (from your database) into a list of `AuthorDto` objects (Data Transfer Objects).

**What is being mapped?**
- **From:** `authors` — a list of `Author` entities retrieved from your database.
- **To:** `IEnumerable<AuthorDto>` — a list of `AuthorDto` objects, which are simplified versions of your entities, designed for sending data to the client.

**Why do we use mapping/DTOs?**
- Entities (like `Author`) often contain extra data, navigation properties, or sensitive information you don't want to expose to the client.
- DTOs (like `AuthorDto`) are designed to contain only the data you want to send to the client—nothing more, nothing less.
- AutoMapper automates the process of copying data from your entities to your DTOs, so you don't have to write repetitive code.

**Example:**

Suppose you have:
```csharp
public class Author {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Biography { get; set; }
    public List<Book> Books { get; set; }
}

public class AuthorDto {
    public int Id { get; set; }
    public string Name { get; set; }
}
```

When you call:
```csharp
var authorDtos = _mapper.Map<IEnumerable<AuthorDto>>(authors);
```
- AutoMapper will create a new list of `AuthorDto` objects, copying the `Id` and `Name` from each `Author` in the `authors` list.

**Summary:**
`_mapper.Map<IEnumerable<AuthorDto>>(authors)` converts a list of `Author` entities into a list of `AuthorDto` objects, making it easy to send only the necessary data to the client.

---

### Tip 3: Database Migration Analogy

A database migration is like updating your house to match new blueprints—without tearing it down. You change your C# models, create a migration (which is a plan for the changes), and apply it so Entity Framework updates your database structure automatically. This keeps your database in sync with your code, safely and step by step.

---

### Tip 4: Understanding Lazy Loading vs. Eager Loading (Why Use `Include`?)

By default, Entity Framework uses **lazy loading** (or sometimes no loading at all for navigation properties, depending on configuration). This means that when you fetch an `Author`, their `Books` collection is **not** automatically loaded from the database. If you try to access `author.Books` later, it might be empty or trigger a separate database query for each author (which is inefficient).

**Why use `Include`?**
- The `Include` method is used for **eager loading** related data. For example:
  ```csharp
  var authors = await _context.Authors
      .Include(a => a.Books)
      .ToListAsync();
  ```
- This tells Entity Framework to load all authors **and** their related books in a single query.
- It prevents the "N+1 query problem" (one query for authors, then one per author for books), making your code more efficient and predictable.

**Summary:**
Use `Include` to eagerly load related entities (like `Books` for each `Author`) in a single database query, improving performance and convenience when you need related data together.

---

## 🗄️ Step 4: Create the Database Context

Create a new folder called `Data` in your `OnlineBookstore` project, then add `AppDbContext.cs`:

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Models;

namespace OnlineBookstore.Data
{
    // IdentityDbContext<ApplicationUser> automatically creates all the database tables needed for user authentication and authorization, and uses your custom ApplicationUser class for user data.
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        // This constructor passes the database options (like connection string and settings) to the base IdentityDbContext, so Entity Framework knows how to connect to and configure your database.
        // The constructor is empty because all the necessary initialization is already done by the base class when you pass options to it. No extra code is needed unless you want to add custom logic.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet properties for our entities
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships using Fluent API
            ConfigureBookRelationships(modelBuilder);
            ConfigureOrderRelationships(modelBuilder);
            ConfigureReviewRelationships(modelBuilder);
            ConfigureShoppingCartRelationships(modelBuilder);
        }

        private void ConfigureBookRelationships(ModelBuilder modelBuilder)
        {
            // Book -> Author (Many-to-One)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict); // What it does: Prevents deleting a parent record if it has child records.

            // Book -> Publisher (Many-to-One)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Book -> Category (Many-to-One)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Book entity constraints
            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Book>()
                .Property(b => b.Title)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Book>()
                .Property(b => b.StockQuantity)
                .HasDefaultValue(0);
        }

        private void ConfigureOrderRelationships(ModelBuilder modelBuilder)
        {
            // Order -> ApplicationUser (Many-to-One)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItem -> Order (Many-to-One)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem -> Book (Many-to-One)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Book)
                .WithMany()
                .HasForeignKey(oi => oi.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Order entity constraints
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderStatus)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingAddress)
                .HasMaxLength(500)
                .IsRequired();

            // Configure OrderItem entity constraints
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");
        }

        private void ConfigureReviewRelationships(ModelBuilder modelBuilder)
        {
            // Review -> Book (Many-to-One)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review -> ApplicationUser (Many-to-One)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Review entity constraints
            modelBuilder.Entity<Review>()
                .Property(r => r.Rating)
                .HasDefaultValue(1);

            modelBuilder.Entity<Review>()
                .Property(r => r.Comment)
                .HasMaxLength(1000);

            // Ensure one review per user per book
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.BookId })
                .IsUnique();
        }

        private void ConfigureShoppingCartRelationships(ModelBuilder modelBuilder)
        {
            // ShoppingCartItem -> ApplicationUser (Many-to-One)
            modelBuilder.Entity<ShoppingCartItem>()
                .HasOne(sci => sci.User)
                .WithMany(u => u.ShoppingCartItems)
                .HasForeignKey(sci => sci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ShoppingCartItem -> Book (Many-to-One)
            modelBuilder.Entity<ShoppingCartItem>()
                .HasOne(sci => sci.Book)
                .WithMany()
                .HasForeignKey(sci => sci.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ShoppingCartItem entity constraints
            modelBuilder.Entity<ShoppingCartItem>()
                .Property(sci => sci.Quantity)
                .HasDefaultValue(1);

            // Ensure one cart item per user per book
            modelBuilder.Entity<ShoppingCartItem>()
                .HasIndex(sci => new { sci.UserId, sci.BookId })
                .IsUnique();
        }
    }
}
```

**💡 What is `DbContextOptions<AppDbContext> options`?**
`DbContextOptions` contains all the configuration settings for your database (connection string, database provider, etc.). It's like the "settings file" that tells Entity Framework how to connect to your database. The `options` parameter is automatically provided by the dependency injection system when you register the DbContext in `Program.cs`.

**💡 Why `IdentityDbContext<ApplicationUser>`?**
`IdentityDbContext` is a specialized database context that automatically creates all the user management tables (AspNetUsers, AspNetRoles, AspNetUserRoles, etc.) that you'd normally have to create manually. By extending it with our `ApplicationUser`, we get all the authentication and authorization database tables for free, plus our custom user properties!

---

## ⚙️ Step 4: Configure Connection String

Update your `appsettings.json` file:

```json
{
  "ConnectionStrings": {
    // the code below will check if the database exists If it doesn't exist, create it automatically
    "DefaultConnection": "Server=servername;Database=databasename;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
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

## 🔧 Step 5: Register DbContext in Program.cs

Update your `Program.cs` file to register the DbContext:

```csharp
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.Models;
using Microsoft.AspNetCore.Identity;

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

app.Run();
```

---

## 🗃️ Step 6: Create and Apply Migrations

### **Create Initial Migration:**
```bash
dotnet ef migrations add InitialCreate
```

### **Apply Migration to Database:**
```bash
dotnet ef database update
```

---

## 🌱 Step 7: Create Database Seeder

Create a new file `Data/DbInitializer.cs`:

```csharp
using OnlineBookstore.Models;
using Microsoft.AspNetCore.Identity;

namespace OnlineBookstore.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder)
        {
            // This line creates a temporary workspace that gives you access to all your application's services (like DbContext and UserManager) so you can use them to seed data into your database.
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            // This line gets the database context (AppDbContext) from the service container so you can add, read, update, or delete data in your database.
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            // This line gets the user manager service so you can create, update, or manage user accounts and their roles in your application.
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            // This line gets the role manager service so you can create and manage user roles (like Admin, User) in your application.
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Fiction" },
                    new Category { Name = "Non-Fiction" },
                    new Category { Name = "Science Fiction" },
                    new Category { Name = "Mystery & Thriller" },
                    new Category { Name = "Romance" },
                    new Category { Name = "Biography" },
                    new Category { Name = "History" },
                    new Category { Name = "Self-Help" },
                    new Category { Name = "Business" },
                    new Category { Name = "Technology" }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Authors
            if (!context.Authors.Any())
            {
                var authors = new List<Author>
                {
                    new Author { Name = "J.K. Rowling", Biography = "British author best known for the Harry Potter series" },
                    new Author { Name = "Stephen King", Biography = "American author of horror, supernatural fiction, suspense, and fantasy novels" },
                    new Author { Name = "Agatha Christie", Biography = "English writer known for her detective novels" },
                    new Author { Name = "Dan Brown", Biography = "American author best known for his thriller novels" },
                    new Author { Name = "John Grisham", Biography = "American novelist, attorney, politician, and activist" },
                    new Author { Name = "Nora Roberts", Biography = "American author of more than 225 romance novels" },
                    new Author { Name = "James Patterson", Biography = "American author and philanthropist" },
                    new Author { Name = "Suzanne Collins", Biography = "American television writer and novelist" },
                    new Author { Name = "Veronica Roth", Biography = "American novelist and short story writer" },
                    new Author { Name = "George R.R. Martin", Biography = "American novelist and short story writer" }
                };

                context.Authors.AddRange(authors);
                await context.SaveChangesAsync();
            }

            // Seed Publishers
            if (!context.Publishers.Any())
            {
                var publishers = new List<Publisher>
                {
                    new Publisher { Name = "Penguin Random House", Description = "One of the world's leading book publishers" },
                    new Publisher { Name = "HarperCollins", Description = "One of the world's largest publishing companies" },
                    new Publisher { Name = "Simon & Schuster", Description = "American publishing company" },
                    new Publisher { Name = "Macmillan Publishers", Description = "International publishing company" },
                    new Publisher { Name = "Hachette Book Group", Description = "Publishing company owned by Hachette Livre" },
                    new Publisher { Name = "Scholastic", Description = "American multinational publishing, education and media company" },
                    new Publisher { Name = "Bloomsbury", Description = "British publishing house" },
                    new Publisher { Name = "Faber and Faber", Description = "Independent publishing house in London" },
                    new Publisher { Name = "Vintage Books", Description = "Imprint of Penguin Random House" },
                    new Publisher { Name = "Doubleday", Description = "American publishing company" }
                };

                context.Publishers.AddRange(publishers);
                await context.SaveChangesAsync();
            }

            // Seed Books
            if (!context.Books.Any())
            {
                var books = new List<Book>
                {
                    new Book
                    {
                        Title = "Harry Potter and the Philosopher's Stone",
                        Description = "The first novel in the Harry Potter series",
                        Price = 12.99m,
                        StockQuantity = 50,
                        AuthorId = context.Authors.First(a => a.Name == "J.K. Rowling").Id,
                        PublisherId = context.Publishers.First(p => p.Name == "Bloomsbury").Id,
                        CategoryId = context.Categories.First(c => c.Name == "Fiction").Id,
                        CoverImageUrl = "https://example.com/harry-potter-1.jpg"
                    },
                    new Book
                    {
                        Title = "The Shining",
                        Description = "A horror novel by Stephen King",
                        Price = 14.99m,
                        StockQuantity = 30,
                        AuthorId = context.Authors.First(a => a.Name == "Stephen King").Id,
                        PublisherId = context.Publishers.First(p => p.Name == "Penguin Random House").Id,
                        CategoryId = context.Categories.First(c => c.Name == "Mystery & Thriller").Id,
                        CoverImageUrl = "https://example.com/shining.jpg"
                    },
                    new Book
                    {
                        Title = "Murder on the Orient Express",
                        Description = "A detective novel by Agatha Christie",
                        Price = 11.99m,
                        StockQuantity = 25,
                        AuthorId = context.Authors.First(a => a.Name == "Agatha Christie").Id,
                        PublisherId = context.Publishers.First(p => p.Name == "HarperCollins").Id,
                        CategoryId = context.Categories.First(c => c.Name == "Mystery & Thriller").Id,
                        CoverImageUrl = "https://example.com/murder-orient-express.jpg"
                    },
                    new Book
                    {
                        Title = "The Da Vinci Code",
                        Description = "A mystery thriller novel by Dan Brown",
                        Price = 13.99m,
                        StockQuantity = 40,
                        AuthorId = context.Authors.First(a => a.Name == "Dan Brown").Id,
                        PublisherId = context.Publishers.First(p => p.Name == "Doubleday").Id,
                        CategoryId = context.Categories.First(c => c.Name == "Mystery & Thriller").Id,
                        CoverImageUrl = "https://example.com/da-vinci-code.jpg"
                    },
                    new Book
                    {
                        Title = "The Firm",
                        Description = "A legal thriller by John Grisham",
                        Price = 12.99m,
                        StockQuantity = 35,
                        AuthorId = context.Authors.First(a => a.Name == "John Grisham").Id,
                        PublisherId = context.Publishers.First(p => p.Name == "Doubleday").Id,
                        CategoryId = context.Categories.First(c => c.Name == "Mystery & Thriller").Id,
                        CoverImageUrl = "https://example.com/firm.jpg"
                    }
                };

                context.Books.AddRange(books);
                await context.SaveChangesAsync();
            }

            // Seed Roles
            // there's no explicit Roles DbSet in your AppDbContext, but it exists automatically because you inherit from IdentityDbContext<ApplicationUser>!
            if (!context.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Seed Admin User
            // // there's no explicit users DbSet in your AppDbContext, but it exists automatically because you inherit from IdentityDbContext<ApplicationUser>!
            if (!context.Users.Any())
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@bookstore.com",
                    Email = "admin@bookstore.com",
                    EmailConfirmed = true,
                    FullName = "Admin User"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
```

---

## 🔄 Step 8: Call the Seeder in Program.cs

Add this line to your `Program.cs` file before `app.Run();`:

```csharp
// Seed the database
await DbInitializer.SeedAsync(app);
```

Your complete `Program.cs` should look like this:

```csharp
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.Models;
using Microsoft.AspNetCore.Identity;

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

## 🧪 Step 9: Test Your Setup

1. **Create and apply migrations:**
   ```bash
   # Navigate to your project directory
   cd eTickets
   
   # Create initial migration (if not already created)
   dotnet ef migrations add InitialCreate
   
   # Apply migrations to create database tables
   dotnet ef database update
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Check the database:**
   - Open SQL Server Management Studio
   - Connect to your local database
   - Verify that the `eTicketsDb` database was created
   - Check that tables were created with the seeded data

4. **Test the API:**
   - Navigate to `https://localhost:7273/swagger`
   - Test the endpoints to ensure everything is working

---

## 🏆 Best Practices

### **Database Design:**
- Use meaningful table and column names
- Set appropriate data types and constraints
- Use foreign keys for relationships
- Consider indexing for performance

### **Entity Framework:**
- Use Fluent API for complex configurations
- Keep DbContext focused and organized
- Use migrations for database changes
- Seed data for testing and development

### **Security:**
- Use parameterized queries (EF Core handles this)
- Validate input data
- Use proper authentication and authorization
- Keep connection strings secure

---

## ✅ What You've Accomplished

- ✅ Installed Entity Framework Core packages
- ✅ Created and configured the database context
- ✅ Set up all entity relationships using Fluent API
- ✅ Configured connection strings
- ✅ Created and applied database migrations
- ✅ Seeded the database with initial data
- ✅ Set up ASP.NET Identity for authentication

---

## 🚀 Next Steps

Your database is now set up and ready! In the next section, we'll start building the API controllers and implementing the business logic for our online bookstore.

**You've successfully configured your database foundation. Great job!**

---

**Next up:**
- [Section 6: Building and Managing Controllers](./06-BUILDING-CONTROLLERS.md)
