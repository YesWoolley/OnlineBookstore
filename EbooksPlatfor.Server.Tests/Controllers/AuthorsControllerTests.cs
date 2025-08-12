using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineBookstore.Controllers;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;
using OnlineBookstore.Server.Tests.Helpers;
using FluentAssertions;

namespace OnlineBookstore.Tests.Controllers
{
    public class AuthorsControllerTests
    {
        private readonly Mock<IAuthorService> _mockAuthorService;
        private readonly AuthorsController _controller;

        public AuthorsControllerTests()
        {
            _mockAuthorService = new Mock<IAuthorService>();
            _controller = new AuthorsController(_mockAuthorService.Object);
        }

        [Fact]
        public async Task GetAuthors_ReturnsOkResult_WithAuthorsList()
        {
            // Arrange
            var authors = new List<AuthorDto>
            {
                TestHelpers.CreateTestAuthorDto(1),
                TestHelpers.CreateTestAuthorDto(2)
            };
            _mockAuthorService.Setup(x => x.GetAllAuthorsAsync()).ReturnsAsync(authors);

            // Act
            var result = await _controller.GetAuthors();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedAuthors = okResult.Value.Should().BeOfType<List<AuthorDto>>().Subject;
            returnedAuthors.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAuthors_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockAuthorService.Setup(x => x.GetAllAuthorsAsync()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAuthors();

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task GetAuthor_ReturnsOkResult_WhenAuthorExists()
        {
            // Arrange
            var author = TestHelpers.CreateTestAuthorDto(1);
            _mockAuthorService.Setup(x => x.GetAuthorByIdAsync(1)).ReturnsAsync(author);

            // Act
            var result = await _controller.GetAuthor(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedAuthor = okResult.Value.Should().BeOfType<AuthorDto>().Subject;
            returnedAuthor.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetAuthor_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            // Arrange
            _mockAuthorService.Setup(x => x.GetAuthorByIdAsync(999)).ReturnsAsync((AuthorDto?)null);

            // Act
            var result = await _controller.GetAuthor(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateAuthor_ReturnsCreatedAtAction_WhenValidData()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateAuthorDto();
            var createdAuthor = TestHelpers.CreateTestAuthorDto(1);
            _mockAuthorService.Setup(x => x.CreateAuthorAsync(createDto)).ReturnsAsync(createdAuthor);

            // Act
            var result = await _controller.CreateAuthor(createDto);

            // Assert
            var createdAtResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtResult.Value.Should().BeOfType<AuthorDto>();
        }

        [Fact]
        public async Task CreateAuthor_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateAuthorDto();
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.CreateAuthor(createDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateAuthor_ReturnsBadRequest_WhenServiceThrowsArgumentException()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateAuthorDto();
            _mockAuthorService.Setup(x => x.CreateAuthorAsync(createDto))
                .ThrowsAsync(new ArgumentException("Author with this name already exists"));

            // Act
            var result = await _controller.CreateAuthor(createDto);

            // Assert
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task UpdateAuthor_ReturnsNoContent_WhenValidData()
        {
            // Arrange
            var updateDto = TestHelpers.CreateTestUpdateAuthorDto();
            var updatedAuthor = TestHelpers.CreateTestAuthorDto(1);
            _mockAuthorService.Setup(x => x.UpdateAuthorAsync(1, updateDto)).ReturnsAsync(updatedAuthor);

            // Act
            var result = await _controller.UpdateAuthor(1, updateDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAuthor_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var updateDto = TestHelpers.CreateTestUpdateAuthorDto();
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.UpdateAuthor(1, updateDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateAuthor_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            // Arrange
            var updateDto = TestHelpers.CreateTestUpdateAuthorDto();
            _mockAuthorService.Setup(x => x.UpdateAuthorAsync(999, updateDto))
                .ThrowsAsync(new ArgumentException("Author not found"));

            // Act
            var result = await _controller.UpdateAuthor(999, updateDto);

            // Assert
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task DeleteAuthor_ReturnsNoContent_WhenAuthorExists()
        {
            // Arrange
            _mockAuthorService.Setup(x => x.DeleteAuthorAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAuthor(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteAuthor_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            // Arrange
            _mockAuthorService.Setup(x => x.DeleteAuthorAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteAuthor(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteAuthor_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockAuthorService.Setup(x => x.DeleteAuthorAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.DeleteAuthor(1);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }
    }
}
