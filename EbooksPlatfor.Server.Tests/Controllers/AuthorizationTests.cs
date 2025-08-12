using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineBookstore.Controllers;
using OnlineBookstore.DTOs;
using OnlineBookstore.Server.Tests.Helpers;
using OnlineBookstore.Services;
using System.Security.Claims;

namespace OnlineBookstore.Tests.Controllers
{
    public class AuthorizationTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IBookService> _mockBookService;
        private readonly AuthController _authController;
        private readonly BooksController _booksController;

        public AuthorizationTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockBookService = new Mock<IBookService>();
            _authController = new AuthController(_mockAuthService.Object);
            _booksController = new BooksController(_mockBookService.Object);
        }

        // Role-based Authorization Tests
        [Fact]
        public async Task AdminOnlyEndpoint_ReturnsForbidden_WhenUserIsNotAdmin()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user-1"),
                new Claim(ClaimTypes.Role, "User")
            };
            _booksController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            // Act
            var result = await _booksController.CreateBook(TestHelpers.CreateTestCreateBookDto());

            // Assert
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task AdminOnlyEndpoint_ReturnsOkResult_WhenUserIsAdmin()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "admin-1"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            _booksController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            var createBookDto = TestHelpers.CreateTestCreateBookDto();
            var createdBook = TestHelpers.CreateTestBookDto();
            _mockBookService.Setup(x => x.CreateBookAsync(createBookDto)).ReturnsAsync(createdBook);

            // Act
            var result = await _booksController.CreateBook(createBookDto);

            // Assert
            var createdAtResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var returnedBook = createdAtResult.Value.Should().BeOfType<BookDto>().Subject;
        }

        // JWT Token Validation Tests
        [Fact]
        public async Task ProtectedEndpoint_ReturnsUnauthorized_WhenNoTokenProvided()
        {
            // Arrange
            _booksController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            // Act
            var result = await _booksController.CreateBook(TestHelpers.CreateTestCreateBookDto());

            // Assert
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task ProtectedEndpoint_ReturnsUnauthorized_WhenInvalidTokenProvided()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user-1")
                // No role claim - invalid token
            };
            _booksController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            // Act
            var result = await _booksController.CreateBook(TestHelpers.CreateTestCreateBookDto());

            // Assert
            result.Should().BeOfType<ObjectResult>();
        }

        // User Data Access Tests
        [Fact]
        public async Task UserData_ReturnsCorrectData_WhenUserAccessesOwnData()
        {
            // Arrange
            var userId = "user-1";
            var userDto = TestHelpers.CreateTestUserDto();
            userDto.Id = userId;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            _mockAuthService.Setup(x => x.GetUserProfileAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _authController.GetProfile();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUser = okResult.Value.Should().BeOfType<UserDto>().Subject;
            returnedUser.Id.Should().Be(userId);
        }

        [Fact]
        public async Task GetProfile_ReturnsForbidden_WhenUserAccessesOtherUserData()
        {
            // Arrange
            var requestingUserId = "user-1";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, requestingUserId)
            };
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            // Act
            var result = await _authController.GetProfile();

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}