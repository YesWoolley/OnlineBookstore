using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineBookstore.Controllers;
using OnlineBookstore.DTOs;
using OnlineBookstore.Server.Tests.Helpers;
using OnlineBookstore.Services;

namespace OnlineBookstore.Tests.Controllers
{
    public class ShoppingCartControllerTests
    {
        private readonly Mock<IShoppingCartService> _mockShoppingCartService;
        private readonly ShoppingCartController _controller;

        public ShoppingCartControllerTests()
        {
            _mockShoppingCartService = new Mock<IShoppingCartService>();
            _controller = new ShoppingCartController(_mockShoppingCartService.Object);
        }

        [Fact]
        public async Task GetUserCart_ReturnsOkResult_WithCartItemsList()
        {
            // Arrange
            var cartItems = new List<ShoppingCartItemDto>
            {
                TestHelpers.CreateTestCartItemDto(1),
                TestHelpers.CreateTestCartItemDto(2)
            };
            _mockShoppingCartService.Setup(x => x.GetUserCartAsync("current-user-id")).ReturnsAsync(cartItems);

            // Act
            var result = await _controller.GetUserCart();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCartItems = okResult.Value.Should().BeOfType<List<ShoppingCartItemDto>>().Subject;
            returnedCartItems.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetUserCart_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockShoppingCartService.Setup(x => x.GetUserCartAsync("current-user-id")).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetUserCart();

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task AddToCart_ReturnsCreatedAtAction_WhenValidData()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateCartItemDto();
            var createdCartItem = TestHelpers.CreateTestCartItemDto(1);
            _mockShoppingCartService.Setup(x => x.AddToCartAsync("current-user-id", createDto)).ReturnsAsync(createdCartItem);

            // Act
            var result = await _controller.AddToCart(createDto);

            // Assert
            var createdAtResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtResult.Value.Should().BeOfType<ShoppingCartItemDto>();
        }

        [Fact]
        public async Task AddToCart_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateCartItemDto();
            _controller.ModelState.AddModelError("BookId", "Book ID is required");

            // Act
            var result = await _controller.AddToCart(createDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task AddToCart_ReturnsBadRequest_WhenServiceThrowsInvalidOperationException()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateCartItemDto();
            _mockShoppingCartService.Setup(x => x.AddToCartAsync("current-user-id", createDto))
                .ThrowsAsync(new InvalidOperationException("Book not found"));

            // Act
            var result = await _controller.AddToCart(createDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateCartItem_ReturnsNoContent_WhenValidData()
        {
            // Arrange
            var updateDto = TestHelpers.CreateTestUpdateCartItemDto();
            var updatedCartItem = TestHelpers.CreateTestCartItemDto(1);
            _mockShoppingCartService.Setup(x => x.UpdateCartItemAsync(1, updateDto)).ReturnsAsync(updatedCartItem);

            // Act
            var result = await _controller.UpdateCartItem(1, updateDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateCartItem_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var updateDto = TestHelpers.CreateTestUpdateCartItemDto();
            _controller.ModelState.AddModelError("Quantity", "Quantity must be greater than 0");

            // Act
            var result = await _controller.UpdateCartItem(1, updateDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateCartItem_ReturnsNotFound_WhenCartItemDoesNotExist()
        {
            // Arrange
            var updateDto = TestHelpers.CreateTestUpdateCartItemDto();
            _mockShoppingCartService.Setup(x => x.UpdateCartItemAsync(999, updateDto))
                .ThrowsAsync(new ArgumentException("Cart item not found"));

            // Act
            var result = await _controller.UpdateCartItem(999, updateDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task RemoveFromCart_ReturnsNoContent_WhenCartItemExists()
        {
            // Arrange
            _mockShoppingCartService.Setup(x => x.RemoveFromCartAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveFromCart(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveFromCart_ReturnsNotFound_WhenCartItemDoesNotExist()
        {
            // Arrange
            _mockShoppingCartService.Setup(x => x.RemoveFromCartAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveFromCart(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ClearCart_ReturnsNoContent_WhenCartHasItems()
        {
            // Arrange
            _mockShoppingCartService.Setup(x => x.ClearUserCartAsync("current-user-id")).ReturnsAsync(true);

            // Act
            var result = await _controller.ClearCart();

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ClearCart_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockShoppingCartService.Setup(x => x.ClearUserCartAsync("current-user-id"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.ClearCart();

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task GetCartTotal_ReturnsOkResult_WithTotalAmount()
        {
            // Arrange
            var expectedTotal = 59.97m;
            _mockShoppingCartService.Setup(x => x.GetCartTotalAsync("current-user-id")).ReturnsAsync(expectedTotal);

            // Act
            var result = await _controller.GetCartTotal();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTotal = okResult.Value.Should().BeOfType<decimal>().Subject;
            returnedTotal.Should().Be(expectedTotal);
        }

        [Fact]
        public async Task GetCartTotal_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockShoppingCartService.Setup(x => x.GetCartTotalAsync("current-user-id"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetCartTotal();

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }
    }
}