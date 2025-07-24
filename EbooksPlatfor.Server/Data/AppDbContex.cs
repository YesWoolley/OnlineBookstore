                    using EbooksPlatform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineBookstore.Data
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Publisher> Publishers { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure relationships
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
