# Section 4: Building the Data Structure (Model Design)

Welcome to one of the most important steps in building your Online Bookstore: **designing your data models!**

In this section, we'll identify the core entities for your application, design the C# model classes, and define the relationships between them. This is the foundation for your database and business logic—so let's do it right!

---

## 🧩 Step 1: Identify Core Entities

Let's brainstorm the main things your platform needs to keep track of. For an online bookstore, you'll likely need:

- **ApplicationUser**: The user accounts (customers and admins)
- **Book**: The main product being sold
- **Author**: Who wrote the book
- **Publisher**: Who published the book
- **Category**: Genre or classification
- **Order**: A purchase made by a user
- **OrderItem**: Each book in an order
- **ShoppingCartItem**: Books a user wants to buy
- **Review**: User reviews for books

---

## 📝 Nullability and `[Required]` Best Practice

- Use `[Required]` **only** on non-nullable properties (e.g., `string Title { get; set; } = null!;`).
- Do **not** use `[Required]` on nullable properties (e.g., `string? Description { get; set; }`).
- Nullable properties (with `?`) are always optional and should not be decorated with `[Required]`.
- Navigation properties that are required should be non-nullable and marked with `[Required]`.

**Validation vs Nullable Types:** Database models need C# nullable reference types (`string Title { get; set; } = null!` or `string? Description { get; set; }`) for compile-time null safety, while DTOs need validation attributes (`[Required]`, `[StringLength]`) for runtime API validation. Models focus on structure, DTOs focus on business rules.

---

## 📝 Step 2: Design the C# Model Classes (with Nullability)

Let's create a `Models` folder in your backend project (`OnlineBookstore/Models`) and add a class for each entity. Here's a starting point for each, with required fields and nullability explicitly indicated:

### **ApplicationUser.cs**
```csharp
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        
        // Navigation properties for relationships
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; }
    }
}
```

**💡 Why `IdentityUser`?**
`IdentityUser` contains all the essential user properties (Id, UserName, Email, PasswordHash, etc.) that you'd normally have to create from scratch. By extending it, we only need to add our custom properties (FullName) and navigation properties. This saves us from writing dozens of standard user properties manually!

### **Book.cs**
```csharp
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class Book
    {
        public int Id { get; set; }
        // If you use only [Required] and leave the property uninitialized, you'll get a compiler warning (but your code will still work at runtime).
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; } // For inventory tracking

        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        // A collection (list, set, etc.) of Review objects.
        public ICollection<Review>? Reviews { get; set; }
    }
}
```

### **Author.cs**
```csharp
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Biography { get; set; }
        // A collection (list, set, etc.) of Book objects.
        public ICollection<Book>? Books { get; set; }
    }
}
```

### **Publisher.cs**
```csharp
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class Publisher
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ICollection<Book>? Books { get; set; }
    }
}
```

### **Category.cs**
```csharp
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Book>? Books { get; set; }
    }
}
```

### **Review.cs**
```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public int Rating { get; set; } // 1-5 stars
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

### **Order.cs**
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string OrderStatus { get; set; } = null!; // Pending, Shipped, Delivered, etc.
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
```

### **OrderItem.cs**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
```

### **ShoppingCartItem.cs**
```csharp
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
```

---

## 🔗 Step 3: Define Relationships

- **One-to-Many:**
  - An Author has many Books
  - A Publisher has many Books
  - A Category has many Books
  - A Book has many Reviews
  - An Order has many OrderItems
  - An ApplicationUser has many Orders
  - An ApplicationUser has many Reviews
  - An ApplicationUser has many ShoppingCartItems
- **Many-to-Many (via join tables):**
  - Users and Books (ShoppingCartItems)

**Diagram (simplified):**
```
Author 1---* Book *---1 Publisher
                |
                *
             Category

ApplicationUser 1---* Order 1---* OrderItem *---1 Book
ApplicationUser 1---* Review *---1 Book
ApplicationUser 1---* ShoppingCartItem *---1 Book
```

---

## 🗺️ Step 4: Detailed Relationship Mapping

### **📊 Entity Relationship Overview**

| Entity | Relationships | Type | Foreign Key | Delete Behavior |
|--------|---------------|------|-------------|-----------------|
| **Book** | Author | Many-to-One | `AuthorId` | `Restrict` |
| **Book** | Publisher | Many-to-One | `PublisherId` | `Restrict` |
| **Book** | Category | Many-to-One | `CategoryId` | `Restrict` |
| **Review** | Book | Many-to-One | `BookId` | `Cascade` |
| **Review** | ApplicationUser | Many-to-One | `UserId` | `Restrict` |
| **Order** | ApplicationUser | Many-to-One | `UserId` | `Restrict` |
| **OrderItem** | Order | Many-to-One | `OrderId` | `Cascade` |
| **OrderItem** | Book | Many-to-One | `BookId` | `Restrict` |
| **ShoppingCartItem** | ApplicationUser | Many-to-One | `UserId` | `Cascade` |
| **ShoppingCartItem** | Book | Many-to-One | `BookId` | `Cascade` |

### **🔍 Detailed Relationship Analysis**

#### **📚 Book Relationships (Core Entity)**
```csharp
// Book -> Author (Many-to-One)
// Delete Behavior: RESTRICT (Protect books when deleting author)
Book.AuthorId → Author.Id
Book.Author → Author (Navigation Property)

// Book -> Publisher (Many-to-One)  
// Delete Behavior: RESTRICT (Protect books when deleting publisher)
Book.PublisherId → Publisher.Id
Book.Publisher → Publisher (Navigation Property)

// Book -> Category (Many-to-One)
// Delete Behavior: RESTRICT (Protect books when deleting category)
Book.CategoryId → Category.Id
Book.Category → Category (Navigation Property)

// Book <- Review (One-to-Many)
// Delete Behavior: CASCADE (Delete reviews when book is deleted)
Book.Reviews ← Review.BookId
```

#### **👤 ApplicationUser Relationships (Identity)**
```csharp
// ApplicationUser -> Order (One-to-Many)
// Delete Behavior: RESTRICT (Protect orders when deleting user)
ApplicationUser.Orders ← Order.UserId

// ApplicationUser -> Review (One-to-Many)
// Delete Behavior: RESTRICT (Protect reviews when deleting user)
ApplicationUser.Reviews ← Review.UserId

// ApplicationUser -> ShoppingCartItem (One-to-Many)
// Delete Behavior: CASCADE (Clean cart when user is deleted)
ApplicationUser.ShoppingCartItems ← ShoppingCartItem.UserId
```

#### **🛒 Order Relationships (Business Logic)**
```csharp
// Order -> ApplicationUser (Many-to-One)
// Delete Behavior: RESTRICT (Protect orders when deleting user)
Order.UserId → ApplicationUser.Id
Order.User → ApplicationUser (Navigation Property)

// Order -> OrderItem (One-to-Many)
// Delete Behavior: CASCADE (Delete items when order is deleted)
Order.OrderItems ← OrderItem.OrderId
```

#### **📦 OrderItem Relationships (Transaction Details)**
```csharp
// OrderItem -> Order (Many-to-One)
// Delete Behavior: CASCADE (Delete when order is deleted)
OrderItem.OrderId → Order.Id
OrderItem.Order → Order (Navigation Property)

// OrderItem -> Book (Many-to-One)
// Delete Behavior: RESTRICT (Protect order history when deleting book)
OrderItem.BookId → Book.Id
OrderItem.Book → Book (Navigation Property)
```

#### **🛍️ ShoppingCartItem Relationships (Session Data)**
```csharp
// ShoppingCartItem -> ApplicationUser (Many-to-One)
// Delete Behavior: CASCADE (Clean cart when user is deleted)
ShoppingCartItem.UserId → ApplicationUser.Id
ShoppingCartItem.User → ApplicationUser (Navigation Property)

// ShoppingCartItem -> Book (Many-to-One)
// Delete Behavior: CASCADE (Remove from cart when book is deleted)
ShoppingCartItem.BookId → Book.Id
ShoppingCartItem.Book → Book (Navigation Property)
```

### **🎯 Delete Behavior Strategy**

#### **🛡️ RESTRICT (Data Protection)**
Used when you want to **prevent deletion** of parent records that have dependent children:

- **Author/Publisher/Category → Book**: Protect inventory data
- **ApplicationUser → Order**: Protect order history
- **ApplicationUser → Review**: Protect review integrity
- **Book → OrderItem**: Protect order history

#### **🧹 CASCADE (Cleanup)**
Used when you want to **automatically delete** child records when parent is deleted:

- **Book → Review**: Clean up reviews when book is removed
- **Order → OrderItem**: Clean up order details when order is cancelled
- **ApplicationUser → ShoppingCartItem**: Clean cart when user is deleted
- **Book → ShoppingCartItem**: Remove from carts when book is deleted

### **📋 Navigation Property Patterns**

#### **One-to-Many Navigation Properties**
```csharp
// Parent side (has collection)
public ICollection<Book> Books { get; set; } = new List<Book>();

// Child side (has single reference)
public Author Author { get; set; } = null!;
public int AuthorId { get; set; }
```

#### **Many-to-One Foreign Keys**
```csharp
// Always include both navigation property and foreign key
public int AuthorId { get; set; }        // Foreign key
public Author Author { get; set; } = null!; // Navigation property
```

### **🔧 Fluent API Configuration**
```csharp
// Example: Book -> Author relationship
modelBuilder.Entity<Book>()
    .HasOne(b => b.Author)           // Navigation property
    .WithMany(a => a.Books)          // Collection property
    .HasForeignKey(b => b.AuthorId)   // Foreign key
    .OnDelete(DeleteBehavior.Restrict); // Delete behavior
```

### **💡 Best Practices**

1. **Always include both navigation property and foreign key**
2. **Use meaningful names for foreign keys** (e.g., `AuthorId`, not `Id`)
3. **Choose delete behavior based on business rules**
4. **Use `Restrict` for important data, `Cascade` for cleanup**
5. **Initialize collections to avoid null reference exceptions**

---

## 🏆 Best Practices for Model Design

- **Use navigation properties** for relationships (e.g., `public Author Author { get; set; }`)
- **Use collection properties** for one-to-many (e.g., `public ICollection<Book> Books { get; set; }`)
- **Use meaningful names** for properties and classes
- **Keep models focused**—don't add unrelated properties
- **Add data annotations** (like `[Required]`, `[MaxLength]`) for validation (we'll cover this in detail later)
- **Use nullable reference types** (`?`) for optional fields

---

## ✅ Next Steps

- Add these model classes to your `OnlineBookstore/Models` folder
- In the next section, we'll configure Entity Framework Core, set up the database context, and create our first migration!

**You've just laid the foundation for your entire application. Great job!**

---

**Next up:**
- [Section 5: Database Configuration and Management](./05-DATABASE-CONFIGURATION.md) 