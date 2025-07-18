using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks() 
        {
            try
            {
                var books = await _context.Books 
                    .Include(b => b.Author)      
                    .Include(b => b.Publisher)    
                    .Include(b => b.Category)    
                    .Include(b => b.Reviews)      
                    .ToListAsync();              

                // Input: [{ Id: 1, AuthorId: 5, PublisherId: 3 }]
                // AutoMapper: Applies rules (AuthorId → AuthorName, etc.)
                // Output: [{ Id: 1, AuthorName: "J.K. Rowling", PublisherName: "Scholastic" }]
                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving books", error = ex.Message });
            }
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)  
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Publisher)
                    .Include(b => b.Category)
                    .Include(b => b.Reviews)
                    .FirstOrDefaultAsync(b => b.Id == id);
         

                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                var bookDto = _mapper.Map<BookDto>(book);
                return Ok(bookDto);
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
                var book = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Publisher)
                    .Include(b => b.Category)
                    .Include(b => b.Reviews!)
                        .ThenInclude(r => r.User)  
                    .FirstOrDefaultAsync(b => b.Id == id);
           
                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                var bookDetailDto = _mapper.Map<BookDetailDto>(book);
                return Ok(bookDetailDto);
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

                var book = _mapper.Map<Book>(createBookDto);  
                _context.Books.Add(book);                     
                await _context.SaveChangesAsync();            

                // Reload the book with related data for the response
                await _context.Entry(book)                   
                    .Reference(b => b.Author)                
                    .LoadAsync();                            
                await _context.Entry(book)                   
                    .Reference(b => b.Publisher)             
                    .LoadAsync();                             
                await _context.Entry(book)                   
                    .Reference(b => b.Category)               
                    .LoadAsync();                            
                await _context.Entry(book)                   
                    .Collection(b => b.Reviews!)            
                    .LoadAsync();                            

                var bookDto = _mapper.Map<BookDto>(book);
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, bookDto);
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

                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                _mapper.Map(updateBookDto, book);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the book", error = ex.Message });
            }
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)  // ← DELETE: Use IActionResult (different success responses)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the book", error = ex.Message });
            }
        }

        // GET: api/books/search?query=harry
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDto>>> SearchBooks([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { message = "Search query is required" });
                }

                var books = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Publisher)
                    .Include(b => b.Category)
                    .Include(b => b.Reviews)
                    .Where(b => b.Title.Contains(query) ||
                               b.Author.Name.Contains(query) ||
                               b.Category.Name.Contains(query))
                    .ToListAsync();

                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching books", error = ex.Message });
            }
        }
    }
}