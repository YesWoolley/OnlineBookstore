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
            // Before using context, check for null
            if (context == null)
                throw new InvalidOperationException("AppDbContext is not registered in the service provider.");

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
                    new Category { Name = "Technology" },
                    new Category { Name = "Fantasy" }
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
                    new Publisher { Name = "Doubleday", Description = "American publishing company" },
                    new Publisher { Name = "Little, Brown and Company", Description = "American publishing company founded in 1837" }
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
                        CoverImageUrl = "https://m.media-amazon.com/images/I/91r0dvXhNGL._SY522_.jpg"
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
                        CoverImageUrl = "https://people.com/thmb/io9zlB8sE8Qna-fGDJ3ACOdm8fk=/4000x0/filters:no_upscale():max_bytes(150000):strip_icc():focal(683x1199:685x1201)/The-Shining-by-Stephen-King-100524-6074748161324d57b55d4bdad872976d.jpg"
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
                        CoverImageUrl = "https://th.bing.com/th/id/R.12693485c6e43f8037cfa48a0a57b981?rik=%2b23OEZ6mDj%2f7gg&riu=http%3a%2f%2fwww.impawards.com%2f2017%2fposters%2fmurder_on_the_orient_express_xxlg.jpg&ehk=8pbJqtonqwY70sl3d7JeYsBI0WZ1rMQrkbwjYe1qnoA%3d&risl=&pid=ImgRaw&r=0"
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
                        CoverImageUrl = "https://1.bp.blogspot.com/-heRZKZFQ2SM/XmEZJ-yaz6I/AAAAAAAAAKU/bRxATEZ3j9YKootFz3kKDRBggXfgZdp0QCLcBGAsYHQ/s1600/images%2B%252832%2529.jpeg"
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
                        CoverImageUrl = "https://i.thenile.io/r1000/9780385319058.jpg?r=5f2627ecded76"
                    },
                    new Book
                    {
                        Title = "The Hunger Games",
                        Description = "A dystopian novel about survival and rebellion",
                        Price = 15.99m,
                        StockQuantity = 45,
                        AuthorId = context.Authors.First(a => a.Name == "Suzanne Collins").Id,
                        PublisherId = context.Publishers.First(p => p.Name == "Scholastic").Id,
                        CategoryId = context.Categories.First(c => c.Name == "Science Fiction").Id,
                        CoverImageUrl = "https://cdn.kobo.com/book-images/fcc61f79-6dc3-4578-a49b-9628deb9ae23/1200/1200/False/the-hunger-games-hunger-games-book-one.jpg"
                    },
                    new Book
                    {
                        Title = "Divergent",
                        Description = "A young adult dystopian novel about identity and choice",
                        Price = 14.99m,
                        StockQuantity = 40,
                        AuthorId = context.Authors.First(a => a.Name == "Veronica Roth").Id,
                        PublisherId = context.Publishers.First(p => p.Name == "HarperCollins").Id,
                        CategoryId = context.Categories.First(c => c.Name == "Science Fiction").Id,
                        CoverImageUrl = "https://tse3.mm.bing.net/th/id/OIP.JFGBS-7WABHsbLhUnsGfAQHaLO?r=0&rs=1&pid=ImgDetMain&o=7&rm=3"
                    },
                    new Book
                    {
                        Title = "A Game of Thrones",
                        Description = "The first novel in the epic fantasy series",
                        Price = 18.99m,
                        StockQuantity = 30,
                        AuthorId = context.Authors.First(a => a.Name == "George R.R. Martin").Id,
                        PublisherId = context.Publishers.First(p => p.Name == "Vintage Books").Id,
                        CategoryId = context.Categories.First(c => c.Name == "Fantasy").Id,
                        CoverImageUrl = "https://i.harperapps.com/hcanz/covers/9780007459483/x960.jpg"
                    },
                    new Book
                    {
                        Title = "The Notebook",
                        Description = "A romantic novel about enduring love",
                        Price = 11.99m,
                        StockQuantity = 55,
                        AuthorId = context.Authors.First(a => a.Name == "Nora Roberts").Id,
                        PublisherId = context.Publishers.First(p => p.Name == "Simon & Schuster").Id,
                        CategoryId = context.Categories.First(c => c.Name == "Romance").Id,
                        CoverImageUrl = "https://www.themoviedb.org/t/p/original/vNc2TvvibEe5Pls8wJOD7GzM5p2.jpg"
                    },
                    new Book
                    {
                        Title = "Along Came a Spider",
                        Description = "A psychological thriller featuring detective Alex Cross",
                        Price = 13.99m,
                        StockQuantity = 38,
                        AuthorId = context.Authors.First(a => a.Name == "James Patterson").Id,
                        PublisherId = context.Publishers.First(p => p.Name == "Little, Brown and Company").Id,
                        CategoryId = context.Categories.First(c => c.Name == "Mystery & Thriller").Id,
                        CoverImageUrl = "https://m.media-amazon.com/images/I/61qc9RAZVDL.jpg"
                    }
                };

                context.Books.AddRange(books);
                await context.SaveChangesAsync();
            }

            // Seed Roles
            if (roleManager != null && !context.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Seed Admin User
            if (userManager != null && !context.Users.Any())
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@bookstore.com",
                    Email = "admin@bookstore.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User"
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