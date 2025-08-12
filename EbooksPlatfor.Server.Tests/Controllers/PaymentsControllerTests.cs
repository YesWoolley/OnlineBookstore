using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineBookstore.Controllers;
using OnlineBookstore.Services;
using OnlineBookstore.Server.Tests.Helpers;
using FluentAssertions;
using PayPalCheckoutSdk.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OnlineBookstore.Tests.Controllers
{
    public class PaymentsControllerTests
    {
        private readonly Mock<IPayPalService> _mockPayPalService;
        private readonly PaymentsController _controller;

        public PaymentsControllerTests()
        {
            _mockPayPalService = new Mock<IPayPalService>();
            _controller = new PaymentsController(_mockPayPalService.Object);
        }

        [Fact]
        public async Task Success_ReturnsViewResult_WhenValidToken()
        {
            // Arrange
            var token = "EC-123456789";
            var expectedOrder = new Order
            {
                Id = token,
                Status = "COMPLETED"
            };
            _mockPayPalService.Setup(x => x.CaptureOrder(token))
                .ReturnsAsync(expectedOrder);

            // Act
            var result = await _controller.Success(token);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(expectedOrder);
        }

        [Fact]
        public async Task Success_ReturnsViewResult_WhenEmptyToken()
        {
            // Arrange
            var token = "";
            var expectedOrder = new Order
            {
                Id = token,
                Status = "COMPLETED"
            };
            _mockPayPalService.Setup(x => x.CaptureOrder(It.IsAny<string>()))
                .ReturnsAsync(expectedOrder);

            // Act
            var result = await _controller.Success(token);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(expectedOrder);
        }

        [Fact]
        public async Task Success_ReturnsViewResult_WhenNullToken()
        {
            // Arrange
            string? token = null;
            var expectedOrder = new Order
            {
                Id = "",
                Status = "COMPLETED"
            };
            _mockPayPalService.Setup(x => x.CaptureOrder(It.IsAny<string>()))
                .ReturnsAsync(expectedOrder);

            // Act
            var result = await _controller.Success(token!);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(expectedOrder);
        }

        [Fact]
        public async Task Success_ThrowsException_WhenPayPalServiceThrowsException()
        {
            // Arrange
            var token = "EC-123456789";
            _mockPayPalService.Setup(x => x.CaptureOrder(token))
                .ThrowsAsync(new Exception("PayPal capture error"));

            // Act & Assert
            await _controller.Invoking(x => x.Success(token))
                .Should().ThrowAsync<Exception>()
                .WithMessage("PayPal capture error");
        }

        [Fact]
        public async Task Success_ThrowsException_WhenInvalidToken()
        {
            // Arrange
            var token = "INVALID-TOKEN";
            _mockPayPalService.Setup(x => x.CaptureOrder(token))
                .ThrowsAsync(new ArgumentException("Invalid order token"));

            // Act & Assert
            await _controller.Invoking(x => x.Success(token))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid order token");
        }

        [Fact]
        public void Cancel_ReturnsViewResult()
        {
            // Act
            var result = _controller.Cancel();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void Cancel_ReturnsViewResult_WithNoModel()
        {
            // Act
            var result = _controller.Cancel();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeNull();
        }

        [Fact]
        public async Task Success_CallsPayPalServiceWithCorrectToken()
        {
            // Arrange
            var token = "EC-123456789";
            var expectedOrder = new Order { Id = token, Status = "COMPLETED" };
            _mockPayPalService.Setup(x => x.CaptureOrder(token))
                .ReturnsAsync(expectedOrder);

            // Act
            await _controller.Success(token);

            // Assert
            _mockPayPalService.Verify(x => x.CaptureOrder(token), Times.Once);
        }
    }
} 