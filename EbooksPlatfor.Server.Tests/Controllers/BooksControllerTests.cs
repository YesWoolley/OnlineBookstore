 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineBookstore.Controllers;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;
using OnlineBookstore.Server.Tests.Helpers;
using FluentAssertions;

namespace OnlineBookstore.Tests.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockBookService = new Mock<IBookService>();
            _controller = new BooksController(_mockBookService.Object);
        }

        // ===== CRUD OPERATION TESTS =====

        [Fact]
        public async Task GetBooks_ReturnsOkResult_WithBooksList()
        {
            // Arrange
            var books = new List<BookDto>
            {
                TestHelpers.CreateTestBookDto(1),
                TestHelpers.CreateTestBookDto(2)
            };
            _mockBookService.Setup(x => x.GetAllBooksAsync()).ReturnsAsync(books);

            // Act
            var result = await _controller.GetBooks();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBooks = okResult.Value.Should().BeOfType<List<BookDto>>().Subject;
            returnedBooks.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetBook_ReturnsOkResult_WithBook()
        {
            // Arrange
            var book = TestHelpers.CreateTestBookDto(1);
            _mockBookService.Setup(x => x.GetBookByIdAsync(1)).ReturnsAsync(book);

            // Act
            var result = await _controller.GetBook(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBook = okResult.Value.Should().BeOfType<BookDto>().Subject;
            returnedBook.Id.Should().Be(1);
            returnedBook.Title.Should().Be("Test Book 1");
        }

        [Fact]
        public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            _mockBookService.Setup(x => x.GetBookByIdAsync(1)).ReturnsAsync((BookDto?)null);

            // Act
            var result = await _controller.GetBook(1);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateBook_ReturnsCreatedAtAction_WithBook()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateBookDto();
            var createdBook = TestHelpers.CreateTestBookDto(1);
            _mockBookService.Setup(x => x.CreateBookAsync(createDto)).ReturnsAsync(createdBook);

            // Act
            var result = await _controller.CreateBook(createDto);

            // Assert
            var createdAtResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtResult.ActionName.Should().Be(nameof(BooksController.GetBook));
            createdAtResult.RouteValues.Should().NotBeNull();
            createdAtResult.RouteValues!["id"].Should().Be(1);
            var returnedBook = createdAtResult.Value.Should().BeOfType<BookDto>().Subject;
            returnedBook.Id.Should().Be(1);
        }

        [Fact]
        public async Task UpdateBook_ReturnsOkResult_WithUpdatedBook()
        {
            // Arrange
            var updateDto = TestHelpers.CreateTestUpdateBookDto();
            var updatedBook = TestHelpers.CreateTestBookDto(1);
            _mockBookService.Setup(x => x.UpdateBookAsync(1, updateDto)).ReturnsAsync(updatedBook);

            // Act
            var result = await _controller.UpdateBook(1, updateDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var updateDto = TestHelpers.CreateTestUpdateBookDto();
            _mockBookService.Setup(x => x.UpdateBookAsync(999, updateDto))
                .ThrowsAsync(new ArgumentException("Book not found"));

            // Act
            var result = await _controller.UpdateBook(999, updateDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContent_WhenBookExists()
        {
            // Arrange
            _mockBookService.Setup(x => x.DeleteBookAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBook(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            _mockBookService.Setup(x => x.DeleteBookAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBook(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ===== RELATIONSHIP TESTS =====

        [Fact]
        public async Task GetBooksByPublisher_ReturnsOkResult_WithBooksList()
        {
            // Arrange
            var books = new List<BookDto>
            {
                TestHelpers.CreateTestBookDto(1),
                TestHelpers.CreateTestBookDto(2)
            };
            _mockBookService.Setup(x => x.GetBooksByPublisherAsync(1)).ReturnsAsync(books);

            // Act
            var result = await _controller.GetBooksByPublisher(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBooks = okResult.Value.Should().BeOfType<List<BookDto>>().Subject;
            returnedBooks.Should().HaveCount(2);
            returnedBooks.Should().AllSatisfy(book => book.PublisherName.Should().Contain("Test Publisher"));
        }

        [Fact]
        public async Task GetBooksByCategory_ReturnsOkResult_WithBooksList()
        {
            // Arrange
            var books = new List<BookDto>
            {
                TestHelpers.CreateTestBookDto(1),
                TestHelpers.CreateTestBookDto(2)
            };
            _mockBookService.Setup(x => x.GetBooksByCategoryAsync(1)).ReturnsAsync(books);

            // Act
            var result = await _controller.GetBooksByCategory(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBooks = okResult.Value.Should().BeOfType<List<BookDto>>().Subject;
            returnedBooks.Should().HaveCount(2);
            returnedBooks.Should().AllSatisfy(book => book.CategoryName.Should().Contain("Test Category"));
        }

        [Fact]
        public async Task GetBooksByAuthor_ReturnsOkResult_WithBooksList()
        {
            // Arrange
            var books = new List<BookDto>
            {
                TestHelpers.CreateTestBookDto(1),
                TestHelpers.CreateTestBookDto(2)
            };
            _mockBookService.Setup(x => x.GetBooksByAuthorAsync(1)).ReturnsAsync(books);

            // Act
            var result = await _controller.GetBooksByAuthor(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBooks = okResult.Value.Should().BeOfType<List<BookDto>>().Subject;
            returnedBooks.Should().HaveCount(2);
            returnedBooks.Should().AllSatisfy(book => book.AuthorName.Should().Contain("Test Author"));
        }

        // ===== SEARCH AND FILTER TESTS =====

        [Fact]
        public async Task SearchBooks_ReturnsOkResult_WithMatchingBooks()
        {
            // Arrange
            var books = new List<BookDto>
            {
                TestHelpers.CreateTestBookDto(1),
                TestHelpers.CreateTestBookDto(2)
            };
            _mockBookService.Setup(x => x.SearchBooksAsync("test")).ReturnsAsync(books);

            // Act
            var result = await _controller.SearchBooks("test");

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBooks = okResult.Value.Should().BeOfType<List<BookDto>>().Subject;
            returnedBooks.Should().HaveCount(2);
        }

        

        // ===== ERROR HANDLING TESTS =====

        [Fact]
        public async Task GetBooks_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockBookService.Setup(x => x.GetAllBooksAsync()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetBooks();

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task CreateBook_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateBookDto();
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.CreateBook(createDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateBook_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var updateDto = TestHelpers.CreateTestUpdateBookDto();
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.UpdateBook(1, updateDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateBook_ReturnsNotFound_WhenPublisherDoesNotExist()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateBookDto();
            _mockBookService.Setup(x => x.CreateBookAsync(createDto))
                .ThrowsAsync(new ArgumentException("Publisher not found"));

            // Act
            var result = await _controller.CreateBook(createDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateBook_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateBookDto();
            _mockBookService.Setup(x => x.CreateBookAsync(createDto))
                .ThrowsAsync(new ArgumentException("Category not found"));

            // Act
            var result = await _controller.CreateBook(createDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateBook_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            // Arrange
            var createDto = TestHelpers.CreateTestCreateBookDto();
            _mockBookService.Setup(x => x.CreateBookAsync(createDto))
                .ThrowsAsync(new ArgumentException("Author not found"));

            // Act
            var result = await _controller.CreateBook(createDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
