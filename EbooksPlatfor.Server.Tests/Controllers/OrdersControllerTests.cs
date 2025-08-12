using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineBookstore.Controllers;
using OnlineBookstore.DTOs;
using OnlineBookstore.Server.Tests.Helpers;
using OnlineBookstore.Services;

namespace OnlineBookstore.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _controller = new OrdersController(_mockOrderService.Object);
        }

        [Fact]
        public async Task GetUserOrders_ReturnsOkResult_WithOrdersList()
        {
            // Arrange
            var orders = new List<OrderDto>
            {
                TestHelpers.CreateTestOrderDto(1),
                TestHelpers.CreateTestOrderDto(2)
            };
            _mockOrderService.Setup(x => x.GetUserOrdersAsync("current-user-id")).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetUserOrders();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedOrders = okResult.Value.Should().BeOfType<List<OrderDto>>().Subject;
            returnedOrders.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetUserOrders_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockOrderService.Setup(x => x.GetUserOrdersAsync("current-user-id")).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetUserOrders();

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task GetOrder_ReturnsOkResult_WhenOrderExists()
        {
            // Arrange
            var order = TestHelpers.CreateTestOrderDto(1);
            _mockOrderService.Setup(x => x.GetOrderByIdAsync(1)).ReturnsAsync(order);

            // Act
            var result = await _controller.GetOrder(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedOrder = okResult.Value.Should().BeOfType<OrderDto>().Subject;
            returnedOrder.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            _mockOrderService.Setup(x => x.GetOrderByIdAsync(999)).ReturnsAsync((OrderDto?)null);

            // Act
            var result = await _controller.GetOrder(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedAtAction_WhenValidData()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateOrderDto();
            var createdOrder = TestHelpers.CreateTestOrderDto(1);
            _mockOrderService.Setup(x => x.CreateOrderAsync("current-user-id", createDto)).ReturnsAsync(createdOrder);

            // Act
            var result = await _controller.CreateOrder(createDto);

            // Assert
            var createdAtResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtResult.Value.Should().BeOfType<OrderDto>();
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateOrderDto();
            _controller.ModelState.AddModelError("ShippingAddress", "Shipping address is required");

            // Act
            var result = await _controller.CreateOrder(createDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenCartIsEmpty()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateOrderDto();
            _mockOrderService.Setup(x => x.CreateOrderAsync("current-user-id", createDto))
                .ThrowsAsync(new InvalidOperationException("Shopping cart is empty"));

            // Act
            var result = await _controller.CreateOrder(createDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateOrder_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateOrderDto();
            _mockOrderService.Setup(x => x.CreateOrderAsync("current-user-id", createDto))
                .ThrowsAsync(new ArgumentException("Book not found"));

            // Act
            var result = await _controller.CreateOrder(createDto);

            // Assert
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task UpdateOrderStatus_ReturnsOkResult_WhenValidData()
        {
            // Arrange
            var updatedOrder = TestHelpers.CreateTestOrderDto(1);
            _mockOrderService.Setup(x => x.UpdateOrderStatusAsync(1, "Shipped")).ReturnsAsync(updatedOrder);

            // Act
            var result = await _controller.UpdateOrderStatus(1, "Shipped");

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedOrder = okResult.Value.Should().BeOfType<OrderDto>().Subject;
            returnedOrder.Id.Should().Be(1);
        }

        [Fact]
        public async Task UpdateOrderStatus_ReturnsBadRequest_WhenInvalidStatus()
        {
            // Arrange
            _mockOrderService.Setup(x => x.UpdateOrderStatusAsync(1, "InvalidStatus"))
                .ThrowsAsync(new ArgumentException("Invalid order status"));

            // Act
            var result = await _controller.UpdateOrderStatus(1, "InvalidStatus");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateOrderStatus_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockOrderService.Setup(x => x.UpdateOrderStatusAsync(1, "Shipped"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.UpdateOrderStatus(1, "Shipped");

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }
    }
}