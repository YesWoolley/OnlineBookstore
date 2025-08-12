using FluentAssertions;
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
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        // Registration Tests
        [Fact]
        public async Task Register_ReturnsOkResult_WhenValidData()
        {
            // Arrange
            var registerDto = TestHelpers.CreateTestRegisterDto();
            var authResult = TestHelpers.CreateTestAuthResultDto();
            _mockAuthService.Setup(x => x.RegisterAsync(registerDto)).ReturnsAsync(authResult);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedAuthResult = okResult.Value.Should().BeOfType<AuthResultDto>().Subject;
            returnedAuthResult.Success.Should().BeTrue();
            returnedAuthResult.Token.Should().NotBeNullOrEmpty();
            returnedAuthResult.User.Should().NotBeNull();
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var registerDto = TestHelpers.CreateTestRegisterDto();
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserAlreadyExists()
        {
            // Arrange
            var registerDto = TestHelpers.CreateTestRegisterDto();
            var authResult = new AuthResultDto
            {
                Success = false,
                Message = "User with this email already exists"
            };
            _mockAuthService.Setup(x => x.RegisterAsync(registerDto)).ReturnsAsync(authResult);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task Register_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var registerDto = TestHelpers.CreateTestRegisterDto();
            _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        // Login Tests
        [Fact]
        public async Task Login_ReturnsOkResult_WhenValidCredentials()
        {
            // Arrange
            var loginDto = TestHelpers.CreateTestLoginDto();
            var authResult = TestHelpers.CreateTestAuthResultDto();
            _mockAuthService.Setup(x => x.LoginAsync(loginDto)).ReturnsAsync(authResult);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedAuthResult = okResult.Value.Should().BeOfType<AuthResultDto>().Subject;
            returnedAuthResult.Success.Should().BeTrue();
            returnedAuthResult.Token.Should().NotBeNullOrEmpty();
            returnedAuthResult.User.Should().NotBeNull();
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var loginDto = TestHelpers.CreateTestLoginDto();
            _controller.ModelState.AddModelError("UserName", "Username is required");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenInvalidCredentials()
        {
            // Arrange
            var loginDto = TestHelpers.CreateTestLoginDto();
            var authResult = new AuthResultDto
            {
                Success = false,
                Message = "Invalid credentials"
            };
            _mockAuthService.Setup(x => x.LoginAsync(loginDto)).ReturnsAsync(authResult);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task Login_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var loginDto = TestHelpers.CreateTestLoginDto();
            _mockAuthService.Setup(x => x.LoginAsync(loginDto))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        // Refresh Token Tests
        [Fact]
        public async Task RefreshToken_ReturnsOkResult_WhenValidRefreshToken()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            var authResult = TestHelpers.CreateTestAuthResultDto();
            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshToken)).ReturnsAsync(authResult);

            // Act
            var result = await _controller.RefreshToken(refreshToken);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedAuthResult = okResult.Value.Should().BeOfType<AuthResultDto>().Subject;
            returnedAuthResult.Success.Should().BeTrue();
            returnedAuthResult.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task RefreshToken_ReturnsBadRequest_WhenInvalidRefreshToken()
        {
            // Arrange
            var refreshToken = "invalid-refresh-token";
            var authResult = new AuthResultDto
            {
                Success = false,
                Message = "Invalid refresh token"
            };
            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshToken)).ReturnsAsync(authResult);

            // Act
            var result = await _controller.RefreshToken(refreshToken);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task RefreshToken_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshToken))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.RefreshToken(refreshToken);

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        // Logout Tests
        [Fact]
        public async Task Logout_ReturnsOkResult_WhenValidRefreshToken()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            _mockAuthService.Setup(x => x.LogoutAsync(refreshToken)).ReturnsAsync(true);

            // Act
            var result = await _controller.Logout(refreshToken);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task Logout_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            _mockAuthService.Setup(x => x.LogoutAsync(refreshToken))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Logout(refreshToken);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        // Profile Tests
        [Fact]
        public async Task GetProfile_ReturnsOkResult_WhenUserIsAuthenticated()
        {
            // Arrange
            var userId = "user-1";
            var userDto = TestHelpers.CreateTestUserDto();

            // Mock User Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            _mockAuthService.Setup(x => x.GetUserProfileAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _controller.GetProfile();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUser = okResult.Value.Should().BeOfType<UserDto>().Subject;
            returnedUser.Id.Should().Be(userDto.Id);
            returnedUser.Email.Should().Be(userDto.Email);
        }

        [Fact]
        public async Task GetProfile_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.GetProfile();

            // Assert
            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        // Update Profile Tests
        [Fact]
        public async Task UpdateProfile_ReturnsOkResult_WhenValidData()
        {
            // Arrange
            var updateProfileDto = TestHelpers.CreateTestUpdateProfileDto();
            var userId = "user-1";
            var updatedUserDto = TestHelpers.CreateTestUserDto();

            // Mock User Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            _mockAuthService.Setup(x => x.UpdateProfileAsync(userId, updateProfileDto))
                .ReturnsAsync(updatedUserDto);

            // Act
            var result = await _controller.UpdateProfile(updateProfileDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUser = okResult.Value.Should().BeOfType<UserDto>().Subject;
            returnedUser.Id.Should().Be(updatedUserDto.Id);
        }

        [Fact]
        public async Task UpdateProfile_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var updateProfileDto = TestHelpers.CreateTestUpdateProfileDto();
            _controller.ModelState.AddModelError("FirstName", "First name is required");

            // Act
            var result = await _controller.UpdateProfile(updateProfileDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateProfile_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var updateProfileDto = TestHelpers.CreateTestUpdateProfileDto();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.UpdateProfile(updateProfileDto);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        // Password Change Tests
        [Fact]
        public async Task ChangePassword_ReturnsOkResult_WhenValidData()
        {
            // Arrange
            var changePasswordDto = TestHelpers.CreateTestChangePasswordDto();
            var userId = "user-1";

            // Mock User Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            _mockAuthService.Setup(x => x.ChangePasswordAsync(userId, changePasswordDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var changePasswordDto = TestHelpers.CreateTestChangePasswordDto();
            _controller.ModelState.AddModelError("CurrentPassword", "Current password is required");

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenCurrentPasswordIsIncorrect()
        {
            // Arrange
            var changePasswordDto = TestHelpers.CreateTestChangePasswordDto();
            var userId = "user-1";

            // Mock User Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            _mockAuthService.Setup(x => x.ChangePasswordAsync(userId, changePasswordDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        }

        [Fact]
        public async Task ChangePassword_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var changePasswordDto = TestHelpers.CreateTestChangePasswordDto();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}