using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving books", error = ex.Message });
            }
        }

        // GET: api/books/search?q=harry
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDto>>> SearchBooks([FromQuery] string q)
        {
            try
            {
                // Add logging to debug the issue
                Console.WriteLine($"SearchBooks called with query: '{q}'");
                
                // Test basic functionality first
                if (q == null)
                {
                    Console.WriteLine("Query parameter is null");
                    return BadRequest("Search query cannot be null");
                }
                
                if (string.IsNullOrWhiteSpace(q))
                {
                    Console.WriteLine("Search query is null or empty, returning all books");
                    var allBooks = await _bookService.GetAllBooksAsync();
                    return Ok(allBooks);
                }

                if (q.Length < 2)
                {
                    Console.WriteLine($"Search query too short: '{q}' (length: {q.Length})");
                    return BadRequest("Search query must be at least 2 characters long");
                }

                Console.WriteLine($"Calling BookService.SearchBooksAsync with: '{q}'");
                var books = await _bookService.SearchBooksAsync(q);
                Console.WriteLine($"Search returned {books.Count()} books");
                
                return Ok(books);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SearchBooks error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An error occurred while searching books", error = ex.Message });
            }
        }

        // GET: api/books/category/5
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByCategory(int categoryId)
        {
            try
            {
                var books = await _bookService.GetBooksByCategoryAsync(categoryId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving books by category", error = ex.Message });
            }
        }

        // GET: api/books/author/5
        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByAuthor(int authorId)
        {
            try
            {
                var books = await _bookService.GetBooksByAuthorAsync(authorId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving books by author", error = ex.Message });
            }
        }

        // GET: api/books/publisher/5
        [HttpGet("publisher/{publisherId}")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByPublisher(int publisherId)
        {
            try
            {
                var books = await _bookService.GetBooksByPublisherAsync(publisherId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving books by publisher", error = ex.Message });
            }
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);

                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the book", error = ex.Message });
            }
        }

        // GET: api/books/5/detail
        [HttpGet("{id}/detail")]
        public async Task<ActionResult<BookDetailDto>> GetBookDetail(int id)
        {
            try
            {
                var book = await _bookService.GetBookDetailByIdAsync(id);

                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the book details", error = ex.Message });
            }
        }

        // POST: api/books
        [HttpPost]
        public async Task<IActionResult> CreateBook(CreateBookDto createBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var book = await _bookService.CreateBookAsync(createBookDto);
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the book", error = ex.Message });
            }
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDto updateBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _bookService.UpdateBookAsync(id, updateBookDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the book", error = ex.Message });
            }
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var result = await _bookService.DeleteBookAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "Book not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the book", error = ex.Message });
            }
        }
    }
}