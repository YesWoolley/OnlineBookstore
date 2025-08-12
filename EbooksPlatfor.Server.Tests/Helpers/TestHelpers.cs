using EbooksPlatform.Models;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBookstore.Server.Tests.Helpers
{
    public static class TestHelpers
    {
        // Author-related test data creation methods
        public static Author CreateTestAuthor(int id = 1)
        {
            return new Author
            {
                Id = id,
                Name = $"Test Author {id}",
                Biography = $"Test biography for author {id}",
                Books = new List<Book>()
            };
        }
        public static AuthorDto CreateTestAuthorDto(int id = 1)
        {
            return new AuthorDto
            {
                Id = id,
                Name = $"Test Author {id}",
                Biography = $"Test biography for author {id}",
                BookCount = 2
            };
        }

        public static CreateAuthorDto CreateTestCreateAuthorDto(int id = 1)
        {
            return new CreateAuthorDto
            {
                Name = $"Test Author {id}",
                Biography = $"Test biography for author {id}" 
            };
        }

        public static UpdateAuthorDto CreateTestUpdateAuthorDto()
        {
            return new UpdateAuthorDto
            {
                Name = "Updated Test Author",
                Biography = "Updated test biography"
            };
        }

        // Publisher-related test data creation methods
        public static Publisher CreateTestPublisher(int id = 1)
        {
            return new Publisher
            {
                Id = id,
                Name = $"Test Publisher {id}",
                Description = $"Test description for publisher {id}",
                Books = new List<Book>()
            };
        }

        public static PublisherDto CreateTestPublisherDto(int id = 1)
        {
            return new PublisherDto
            {
                Id = id,
                Name = $"Test Publisher {id}",
                Description = $"Test description for publisher {id}",
                BookCount = id * 2  // Simulate book count for testing
            };
        }

        public static CreatePublisherDto CreateTestCreatePublisherDto(int id = 1)
        {
            return new CreatePublisherDto
            {
                Name = $"Test Publisher {id}",
                Description = $"Test description for publisher {id}"
            };
        }

        public static UpdatePublisherDto CreateTestUpdatePublisherDto(int id = 1)
        {
            return new UpdatePublisherDto
            {
                Name = $"Updated Publisher {id}",
                Description = $"Updated description for publisher {id}"
            };
        }

        // ===== CATEGORY HELPERS =====
        public static Category CreateTestCategory(int id = 1)
        {
            return new Category
            {
                Id = id,
                Name = $"Test Category {id}",
                Books = new List<Book>()
            };
        }

        public static CategoryDto CreateTestCategoryDto(int id = 1)
        {
            return new CategoryDto
            {
                Id = id,
                Name = $"Test Category {id}",
                BookCount = id * 3  // Simulate book count for testing
            };
        }

        public static CreateCategoryDto CreateTestCreateCategoryDto(int id = 1)
        {
            return new CreateCategoryDto
            {
                Name = $"Test Category {id}"
            };
        }

        public static UpdateCategoryDto CreateTestUpdateCategoryDto(int id = 1)
        {
            return new UpdateCategoryDto
            {
                Name = $"Updated Category {id}"
            };
        }

        // ===== BOOK HELPERS =====
        public static Book CreateTestBook(int id = 1)
        {
            return new Book
            {
                Id = id,
                Title = $"Test Book {id}",
                Description = $"Test description for book {id}",
                CoverImageUrl = $"https://example.com/covers/book{id}.jpg",
                Price = 19.99m + id,
                StockQuantity = 10 + id,
                AuthorId = id,
                PublisherId = id,
                CategoryId = id,
                Author = CreateTestAuthor(id),
                Publisher = CreateTestPublisher(id),
                Category = CreateTestCategory(id),
                Reviews = new List<Review>()
            };
        }

        public static BookDto CreateTestBookDto(int id = 1)
        {
            return new BookDto
            {
                Id = id,
                Title = $"Test Book {id}",
                Description = $"Test description for book {id}",
                Price = 19.99m + id,
                StockQuantity = 10 + id,
                CoverImageUrl = $"https://example.com/covers/book{id}.jpg",
                AuthorName = $"Test Author {id}",
                PublisherName = $"Test Publisher {id}",
                CategoryName = $"Test Category {id}",
                ReviewCount = id * 2,
                AverageRating = 4.0 + (id * 0.1)
            };
        }

        public static BookDetailDto CreateTestBookDetailDto(int id = 1)
        {
            return new BookDetailDto
            {
                Id = id,
                Title = $"Test Book {id}",
                Description = $"Test description for book {id}",
                Price = 19.99m + id,
                StockQuantity = 10 + id,
                CoverImageUrl = $"https://example.com/covers/book{id}.jpg",
                AuthorName = $"Test Author {id}",
                PublisherName = $"Test Publisher {id}",
                CategoryName = $"Test Category {id}",
                ReviewCount = id * 2,
                AverageRating = 4.0 + (id * 0.1),
                Reviews = new List<ReviewDto>()
            };
        }

        public static CreateBookDto CreateTestCreateBookDto(int id = 1)
        {
            return new CreateBookDto
            {
                Title = $"Test Book {id}",
                Description = $"Test description for book {id}",
                Price = 19.99m + id,
                StockQuantity = 10 + id,
                CoverImageUrl = $"https://example.com/covers/book{id}.jpg",
                AuthorId = id,
                PublisherId = id,
                CategoryId = id
            };
        }

        public static UpdateBookDto CreateTestUpdateBookDto(int id = 1)
        {
            return new UpdateBookDto
            {
                Title = $"Updated Book {id}",
                Description = $"Updated description for book {id}",
                Price = 29.99m + id,
                StockQuantity = 20 + id,
                CoverImageUrl = $"https://example.com/covers/updated-book{id}.jpg",
                AuthorId = id,
                PublisherId = id,
                CategoryId = id
            };
        }

        // ===== REVIEW HELPERS =====
        public static Review CreateTestReview(int id = 1)
        {
            return new Review
            {
                Id = id,
                BookId = 1,
                UserId = "test-user-id",
                Rating = 5,
                Comment = $"Test review {id}",
                CreatedAt = DateTime.UtcNow,
                Book = CreateTestBook(1),
                User = new ApplicationUser { Id = "test-user-id", UserName = "testuser" }
            };
        }

        public static ReviewDto CreateTestReviewDto(int id = 1)
        {
            return new ReviewDto
            {
                Id = id,
                BookId = 1,
                BookTitle = "Test Book 1",
                UserName = "testuser",
                Rating = 5,
                Comment = $"Test review {id}",
                CreatedAt = DateTime.UtcNow
            };
        }

        public static CreateReviewDto CreateTestCreateReviewDto()
        {
            return new CreateReviewDto
            {
                BookId = 1,
                Rating = 4,
                Comment = "Great book!"
            };
        }

        public static UpdateReviewDto CreateTestUpdateReviewDto()
        {
            return new UpdateReviewDto
            {
                Rating = 5,
                Comment = "Excellent book, highly recommended!"
            };
        }

        public static ApplicationUser CreateTestUser(string userId = "test-user-id")
        {
            return new ApplicationUser
            {
                Id = userId,
                UserName = $"testuser{userId}",
                Email = $"test{userId}@example.com",
                FirstName = $"Test{userId}",
                LastName = $"User{userId}",
                CreatedAt = DateTime.Now.AddDays(-10),
                RefreshToken = $"refresh-token-{userId}",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(7),
                Orders = new List<Order>(),
                Reviews = new List<Review>(),
                ShoppingCartItems = new List<ShoppingCartItem>()
            };
        }

        // ===== SHOPPING CART HELPERS =====
        public static ShoppingCartItem CreateTestCartItem(int id = 1)
        {
            return new ShoppingCartItem
            {
                Id = id,
                Quantity = 2,
                UserId = $"user-{id}",
                BookId = id,
                User = CreateTestUser($"user-{id}"),
                Book = CreateTestBook(id)
            };
        }

        public static ShoppingCartItemDto CreateTestCartItemDto(int id = 1)
        {
            return new ShoppingCartItemDto
            {
                Id = id,
                UserName = $"testuser{id}",
                BookId = id,
                BookTitle = $"Test Book {id}",
                BookPrice = 19.99m + id,
                Quantity = 2,
                TotalPrice = (19.99m + id) * 2
            };
        }

        public static CreateShoppingCartItemDto CreateTestCreateCartItemDto(int id = 1)
        {
            return new CreateShoppingCartItemDto
            {
                BookId = id,
                Quantity = 2
            };
        }

        public static UpdateShoppingCartItemDto CreateTestUpdateCartItemDto(int id = 1)
        {
            return new UpdateShoppingCartItemDto
            {
                Quantity = 3
            };
        }

        public static Order CreateTestOrder(int id = 1)
        {
            return new Order
            {
                Id = id,
                UserId = $"user-{id}",
                User = CreateTestUser($"user-{id}"),
                OrderDate = DateTime.UtcNow,
                TotalAmount = 29.99m + (id * 10),
                ShippingAddress = $"123 Test Street, City {id}",
                OrderStatus = "Pending",
                OrderItems = new List<OrderItem>
                {
                    CreateTestOrderItem(id),
                    CreateTestOrderItem(id + 1)
                }
            };
        }

        public static OrderDto CreateTestOrderDto(int id = 1)
        {
            return new OrderDto
            {
                Id = id,
                UserName = $"testuser{id}",
                OrderDate = DateTime.UtcNow,
                TotalAmount = 29.99m + (id * 10),
                ShippingAddress = $"123 Test Street, City {id}",
                OrderStatus = "Pending",
                OrderItems = new List<OrderItemDto>
                {
                    CreateTestOrderItemDto(id),
                    CreateTestOrderItemDto(id + 1)
                }
            };
        }

        public static CreateOrderDto CreateTestCreateOrderDto(int id = 1)
        {
            return new CreateOrderDto
            {
                ShippingAddress = $"123 Test Street, City {id}",
                OrderStatus = "Pending",
                OrderItems = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { BookId = id, Quantity = 2 },
                    new CreateOrderItemDto { BookId = id + 1, Quantity = 1 }
                }
            };
        }

        public static UpdateOrderDto CreateTestUpdateOrderDto(int id = 1)
        {
            return new UpdateOrderDto
            {
                ShippingAddress = $"456 Updated Street, City {id}",
                OrderStatus = "Shipped"
            };
        }

        public static OrderItem CreateTestOrderItem(int id = 1)
        {
            return new OrderItem
            {
                Id = id,
                OrderId = id,
                Order = CreateTestOrder(id),
                BookId = id,
                Book = CreateTestBook(id),
                Quantity = 2,
                UnitPrice = 19.99m + id
            };
        }

        public static OrderItemDto CreateTestOrderItemDto(int id = 1)
        {
            return new OrderItemDto
            {
                Id = id,
                OrderId = id,
                BookId = id,
                BookTitle = $"Test Book {id}",
                Quantity = 2,
                UnitPrice = 19.99m + id,
                TotalPrice = (19.99m + id) * 2
            };
        }

        public static CreateOrderItemDto CreateTestCreateOrderItemDto(int id = 1)
        {
            return new CreateOrderItemDto
            {
                BookId = id,
                Quantity = 2
            };
        }

        public static UpdateOrderItemDto CreateTestUpdateOrderItemDto(int id = 1)
        {
            return new UpdateOrderItemDto
            {
                Quantity = 3
            };
        }

        // ===== USER HELPERS =====
        public static UserDto CreateTestUserDto(int id = 1)
        {
            return new UserDto
            {
                Id = $"user-{id}",
                UserName = $"testuser{id}",
                Email = $"test{id}@example.com",
                FirstName = $"Test{id}",
                LastName = $"User{id}",
                PhoneNumber = $"+123456789{id:D2}",
                CreatedAt = DateTime.Now.AddDays(-id),
                Roles = new List<string> { "User" }
            };
        }

        public static RegisterDto CreateTestRegisterDto(int id = 1)
        {
            return new RegisterDto
            {
                UserName = $"testuser{id}",
                Email = $"test{id}@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = $"Test{id}",
                LastName = $"User{id}",
                PhoneNumber = $"+123456789{id:D2}"
            };
        }

        public static LoginDto CreateTestLoginDto(int id = 1)
        {
            return new LoginDto
            {
                UserName = $"testuser{id}",
                Password = "Password123!"
            };
        }

        public static AuthResultDto CreateTestAuthResultDto(int id = 1)
        {
            return new AuthResultDto
            {
                Success = true,
                Token = $"jwt-token-{id}",
                RefreshToken = $"refresh-token-{id}",
                User = CreateTestUserDto(id),
                Message = "Authentication successful"
            };
        }

        public static UpdateProfileDto CreateTestUpdateProfileDto(int id = 1)
        {
            return new UpdateProfileDto
            {
                FirstName = $"Updated{id}",
                LastName = $"User{id}",
                PhoneNumber = $"+198765432{id:D2}"
            };
        }

        public static ChangePasswordDto CreateTestChangePasswordDto()
        {
            return new ChangePasswordDto
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };
        }

        // authorization
        public static ClaimsPrincipal CreateTestUserClaims(string userId, string role = "User")
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Role, role),
        new Claim(ClaimTypes.Name, $"testuser{userId}")
    };
            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }

        public static ClaimsPrincipal CreateTestAdminClaims(string userId = "admin-1")
        {
            return CreateTestUserClaims(userId, "Admin");
        }

        public static ClaimsPrincipal CreateTestUnauthenticatedClaims()
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        // ===== PAYMENT HELPERS =====
        public static CreatePaymentDto CreateTestCreatePaymentDto(int id = 1)
        {
            return new CreatePaymentDto
            {
                Amount = 29.99m + id,
                Currency = "USD",
                Description = $"Test payment {id}"
            };
        }

        public static PaymentResultDto CreateTestPaymentResultDto(int id = 1)
        {
            return new PaymentResultDto
            {
                Success = true,
                OrderId = $"EC-{id:D6}",
                ApprovalUrl = $"https://www.sandbox.paypal.com/checkoutnow?token=EC-{id:D6}",
                Status = "PENDING",
                Message = "Payment created successfully"
            };
        }

        public static PaymentCaptureDto CreateTestPaymentCaptureDto(int id = 1)
        {
            return new PaymentCaptureDto
            {
                Success = true,
                OrderId = $"EC-{id:D6}",
                Status = "COMPLETED",
                Amount = 29.99m + id,
                Currency = "USD",
                Message = "Payment captured successfully"
            };
        }

        public static CreatePaymentDto CreateTestCreatePaymentDtoWithLargeAmount(int id = 1)
        {
            return new CreatePaymentDto
            {
                Amount = 999.99m + id,
                Currency = "USD",
                Description = $"Large test payment {id}"
            };
        }

        public static CreatePaymentDto CreateTestCreatePaymentDtoWithZeroAmount(int id = 1)
        {
            return new CreatePaymentDto
            {
                Amount = 0m,
                Currency = "USD",
                Description = $"Zero amount test payment {id}"
            };
        }

        public static PaymentResultDto CreateTestFailedPaymentResultDto(int id = 1)
        {
            return new PaymentResultDto
            {
                Success = false,
                OrderId = $"EC-{id:D6}",
                ApprovalUrl = null,
                Status = "FAILED",
                Message = "Payment failed due to insufficient funds"
            };
        }

        public static PaymentCaptureDto CreateTestFailedPaymentCaptureDto(int id = 1)
        {
            return new PaymentCaptureDto
            {
                Success = false,
                OrderId = $"EC-{id:D6}",
                Status = "FAILED",
                Amount = 29.99m + id,
                Currency = "USD",
                Message = "Payment capture failed"
            };
        }
    }
}

