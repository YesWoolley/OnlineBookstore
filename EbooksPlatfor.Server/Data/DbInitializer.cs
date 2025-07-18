using Microsoft.AspNetCore.Identity;
using OnlineBookstore.Models;

namespace OnlineBookstore.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed Categories
            if(!context.Categories.Any())
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
            if (!context.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Seed Admin User
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