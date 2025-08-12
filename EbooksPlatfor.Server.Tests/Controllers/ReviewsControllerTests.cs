using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineBookstore.Controllers;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;
using OnlineBookstore.Server.Tests.Helpers;
using FluentAssertions;

namespace OnlineBookstore.Tests.Controllers
{
    public class ReviewsControllerTests
    {
        private readonly Mock<IReviewService> _mockReviewService;
        private readonly ReviewsController _controller;

        public ReviewsControllerTests()
        {
            _mockReviewService = new Mock<IReviewService>();
            _controller = new ReviewsController(_mockReviewService.Object);
        }

        [Fact]
        public async Task GetReviews_ReturnsOkResult_WithReviewsList()
        {
            // Arrange
            var reviews = new List<ReviewDto>
            {
                TestHelpers.CreateTestReviewDto(1),
                TestHelpers.CreateTestReviewDto(2)
            };
            _mockReviewService.Setup(x => x.GetAllReviewsAsync()).ReturnsAsync(reviews);

            // Act
            var result = await _controller.GetReviews();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedReviews = okResult.Value.Should().BeOfType<List<ReviewDto>>().Subject;
            returnedReviews.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetReviews_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockReviewService.Setup(x => x.GetAllReviewsAsync()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetReviews();

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task GetReview_ReturnsOkResult_WhenReviewExists()
        {
            // Arrange
            var review = TestHelpers.CreateTestReviewDto(1);
            _mockReviewService.Setup(x => x.GetReviewByIdAsync(1)).ReturnsAsync(review);

            // Act
            var result = await _controller.GetReview(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedReview = okResult.Value.Should().BeOfType<ReviewDto>().Subject;
            returnedReview.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetReview_ReturnsNotFound_WhenReviewDoesNotExist()
        {
            // Arrange
            _mockReviewService.Setup(x => x.GetReviewByIdAsync(999)).ReturnsAsync((ReviewDto?)null);

            // Act
            var result = await _controller.GetReview(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetReviewsByBook_ReturnsOkResult_WithReviewsList()
        {
            // Arrange
            var reviews = new List<ReviewDto>
            {
                TestHelpers.CreateTestReviewDto(1),
                TestHelpers.CreateTestReviewDto(2)
            };
            _mockReviewService.Setup(x => x.GetReviewsByBookAsync(1)).ReturnsAsync(reviews);

            // Act
            var result = await _controller.GetReviewsByBook(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedReviews = okResult.Value.Should().BeOfType<List<ReviewDto>>().Subject;
            returnedReviews.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetReviewsByBook_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockReviewService.Setup(x => x.GetReviewsByBookAsync(1)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetReviewsByBook(1);

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task GetUserReviews_ReturnsOkResult_WithUserReviews()
        {
            // Arrange
            var reviews = new List<ReviewDto>
            {
                TestHelpers.CreateTestReviewDto(1),
                TestHelpers.CreateTestReviewDto(2)
            };
            _mockReviewService.Setup(x => x.GetReviewsByUserAsync("current-user-id")).ReturnsAsync(reviews);

            // Act
            var result = await _controller.GetUserReviews();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedReviews = okResult.Value.Should().BeOfType<List<ReviewDto>>().Subject;
            returnedReviews.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAverageRating_ReturnsOkResult_WithRating()
        {
            // Arrange
            var averageRating = 4.5;
            _mockReviewService.Setup(x => x.GetAverageRatingAsync(1)).ReturnsAsync(averageRating);

            // Act
            var result = await _controller.GetAverageRating(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedRating = okResult.Value.Should().BeOfType<double>().Subject;
            returnedRating.Should().Be(averageRating);
        }

        [Fact]
        public async Task GetAverageRating_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockReviewService.Setup(x => x.GetAverageRatingAsync(1)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAverageRating(1);

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task GetReviewCount_ReturnsOkResult_WithCount()
        {
            // Arrange
            var count = 5;
            _mockReviewService.Setup(x => x.GetReviewCountAsync(1)).ReturnsAsync(count);

            // Act
            var result = await _controller.GetReviewCount(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCount = okResult.Value.Should().BeOfType<int>().Subject;
            returnedCount.Should().Be(count);
        }

        [Fact]
        public async Task HasUserReviewedBook_ReturnsOkResult_WithBoolean()
        {
            // Arrange
            var hasReviewed = true;
            _mockReviewService.Setup(x => x.HasUserReviewedBookAsync("current-user-id", 1)).ReturnsAsync(hasReviewed);

            // Act
            var result = await _controller.HasUserReviewedBook(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBoolean = okResult.Value.Should().BeOfType<bool>().Subject;
            returnedBoolean.Should().Be(hasReviewed);
        }

        [Fact]
        public async Task CreateReview_ReturnsCreatedAtAction_WhenValidData()
        {
            // Arrange
            var createReviewDto = TestHelpers.CreateTestCreateReviewDto();
            var createdReview = TestHelpers.CreateTestReviewDto(1);
            _mockReviewService.Setup(x => x.CreateReviewAsync("current-user-id", createReviewDto)).ReturnsAsync(createdReview);

            // Act
            var result = await _controller.CreateReview(createReviewDto);

            // Assert
            var createdAtResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var returnedReview = createdAtResult.Value.Should().BeOfType<ReviewDto>().Subject;
            returnedReview.Id.Should().Be(1);
        }

        [Fact]
        public async Task CreateReview_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createReviewDto = TestHelpers.CreateTestCreateReviewDto();
            _controller.ModelState.AddModelError("Rating", "Rating is required");

            // Act
            var result = await _controller.CreateReview(createReviewDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateReview_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var createReviewDto = TestHelpers.CreateTestCreateReviewDto();
            _mockReviewService.Setup(x => x.CreateReviewAsync("current-user-id", createReviewDto))
                .ThrowsAsync(new ArgumentException("Book not found"));

            // Act
            var result = await _controller.CreateReview(createReviewDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateReview_ReturnsBadRequest_WhenUserAlreadyReviewed()
        {
            // Arrange
            var createReviewDto = TestHelpers.CreateTestCreateReviewDto();
            _mockReviewService.Setup(x => x.CreateReviewAsync("current-user-id", createReviewDto))
                .ThrowsAsync(new InvalidOperationException("User has already reviewed this book"));

            // Act
            var result = await _controller.CreateReview(createReviewDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateReview_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var createReviewDto = TestHelpers.CreateTestCreateReviewDto();
            _mockReviewService.Setup(x => x.CreateReviewAsync("current-user-id", createReviewDto))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.CreateReview(createReviewDto);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task UpdateReview_ReturnsNoContent_WhenValidData()
        {
            // Arrange
            var updateReviewDto = TestHelpers.CreateTestUpdateReviewDto();
            var updatedReview = TestHelpers.CreateTestReviewDto(1);
            _mockReviewService.Setup(x => x.UpdateReviewAsync(1, "current-user-id", updateReviewDto)).ReturnsAsync(updatedReview);

            // Act
            var result = await _controller.UpdateReview(1, updateReviewDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateReview_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var updateReviewDto = TestHelpers.CreateTestUpdateReviewDto();
            _controller.ModelState.AddModelError("Rating", "Rating is required");

            // Act
            var result = await _controller.UpdateReview(1, updateReviewDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateReview_ReturnsNotFound_WhenReviewDoesNotExist()
        {
            // Arrange
            var updateReviewDto = TestHelpers.CreateTestUpdateReviewDto();
            _mockReviewService.Setup(x => x.UpdateReviewAsync(999, "current-user-id", updateReviewDto))
                .ThrowsAsync(new ArgumentException("Review not found"));

            // Act
            var result = await _controller.UpdateReview(999, updateReviewDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateReview_ReturnsUnauthorized_WhenUserNotOwner()
        {
            // Arrange
            var updateReviewDto = TestHelpers.CreateTestUpdateReviewDto();
            _mockReviewService.Setup(x => x.UpdateReviewAsync(1, "current-user-id", updateReviewDto))
                .ThrowsAsync(new UnauthorizedAccessException("User is not the owner of this review"));

            // Act
            var result = await _controller.UpdateReview(1, updateReviewDto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task UpdateReview_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var updateReviewDto = TestHelpers.CreateTestUpdateReviewDto();
            _mockReviewService.Setup(x => x.UpdateReviewAsync(1, "current-user-id", updateReviewDto))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.UpdateReview(1, updateReviewDto);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task DeleteReview_ReturnsNoContent_WhenReviewExists()
        {
            // Arrange
            _mockReviewService.Setup(x => x.DeleteReviewAsync(1, "current-user-id")).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteReview(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteReview_ReturnsNotFound_WhenReviewDoesNotExist()
        {
            // Arrange
            _mockReviewService.Setup(x => x.DeleteReviewAsync(999, "current-user-id")).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteReview(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteReview_ReturnsUnauthorized_WhenUserNotOwner()
        {
            // Arrange
            _mockReviewService.Setup(x => x.DeleteReviewAsync(1, "current-user-id"))
                .ThrowsAsync(new UnauthorizedAccessException("User is not the owner of this review"));

            // Act
            var result = await _controller.DeleteReview(1);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task DeleteReview_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockReviewService.Setup(x => x.DeleteReviewAsync(1, "current-user-id"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.DeleteReview(1);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }
    }
}