using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineBookstore.Controllers;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;
using OnlineBookstore.Server.Tests.Helpers;
using FluentAssertions;

namespace OnlineBookstore.Tests.Controllers
{
    public class PublishersControllerTests
    {
        private readonly Mock<IPublisherService> _mockPublisherService;
        private readonly PublishersController _controller;

        public PublishersControllerTests()
        {
            _mockPublisherService = new Mock<IPublisherService>();
            _controller = new PublishersController(_mockPublisherService.Object);
        }

        [Fact]
        public async Task GetPublishers_ReturnsOkResult_WithPublishersList()
        {
            // Arrange
            var publishers = new List<PublisherDto>
            {
                TestHelpers.CreateTestPublisherDto(1),
                TestHelpers.CreateTestPublisherDto(2)
            };
            _mockPublisherService.Setup(x => x.GetAllPublishersAsync()).ReturnsAsync(publishers);

            // Act
            var result = await _controller.GetPublishers();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedPublishers = okResult.Value.Should().BeOfType<List<PublisherDto>>().Subject;
            returnedPublishers.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetPublishers_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockPublisherService.Setup(x => x.GetAllPublishersAsync()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetPublishers();

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task GetPublisher_ReturnsOkResult_WhenPublisherExists()
        {
            // Arrange
            var publisher = TestHelpers.CreateTestPublisherDto(1);
            _mockPublisherService.Setup(x => x.GetPublisherByIdAsync(1)).ReturnsAsync(publisher);

            // Act
            var result = await _controller.GetPublisher(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedPublisher = okResult.Value.Should().BeOfType<PublisherDto>().Subject;
            returnedPublisher.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetPublisher_ReturnsNotFound_WhenPublisherDoesNotExist()
        {
            // Arrange
            _mockPublisherService.Setup(x => x.GetPublisherByIdAsync(999)).ReturnsAsync((PublisherDto?)null);

            // Act
            var result = await _controller.GetPublisher(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreatePublisher_ReturnsCreatedAtAction_WhenValidData()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreatePublisherDto();
            var createdPublisher = TestHelpers.CreateTestPublisherDto(1);
            _mockPublisherService.Setup(x => x.CreatePublisherAsync(createDto)).ReturnsAsync(createdPublisher);

            // Act
            var result = await _controller.CreatePublisher(createDto);

            // Assert
            var createdAtResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtResult.Value.Should().BeOfType<PublisherDto>();
        }

        [Fact]
        public async Task CreatePublisher_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreatePublisherDto();
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.CreatePublisher(createDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreatePublisher_ReturnsBadRequest_WhenServiceThrowsArgumentException()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreatePublisherDto();
            _mockPublisherService.Setup(x => x.CreatePublisherAsync(createDto))
                .ThrowsAsync(new ArgumentException("Publisher with this name already exists"));

            // Act
            var result = await _controller.CreatePublisher(createDto);

            // Assert
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task UpdatePublisher_ReturnsNoContent_WhenValidData()
        {
            // Arrange
            var updateDto = TestHelpers.CreateTestUpdatePublisherDto();
            var updatedPublisher = TestHelpers.CreateTestPublisherDto(1);
            _mockPublisherService.Setup(x => x.UpdatePublisherAsync(1, updateDto)).ReturnsAsync(updatedPublisher);

            // Act
            var result = await _controller.UpdatePublisher(1, updateDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdatePublisher_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var updateDto = TestHelpers.CreateTestUpdatePublisherDto();
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.UpdatePublisher(1, updateDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdatePublisher_ReturnsNotFound_WhenPublisherDoesNotExist()
        {
            // Arrange
            var updateDto = TestHelpers.CreateTestUpdatePublisherDto();
            _mockPublisherService.Setup(x => x.UpdatePublisherAsync(999, updateDto))
                .ThrowsAsync(new ArgumentException("Publisher not found"));

            // Act
            var result = await _controller.UpdatePublisher(999, updateDto);

            // Assert
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task DeletePublisher_ReturnsNoContent_WhenPublisherExists()
        {
            // Arrange
            _mockPublisherService.Setup(x => x.DeletePublisherAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePublisher(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeletePublisher_ReturnsNotFound_WhenPublisherDoesNotExist()
        {
            // Arrange
            _mockPublisherService.Setup(x => x.DeletePublisherAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeletePublisher(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeletePublisher_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockPublisherService.Setup(x => x.DeletePublisherAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.DeletePublisher(1);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }
    }
}