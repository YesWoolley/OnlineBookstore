using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        //private readonly AppDbContext _context;
        //private readonly IMapper _mapper;
        private readonly IAuthorService _authorService;

        //public AuthorsController(AppDbContext context, IMapper mapper)
        public AuthorsController(IAuthorService authorService)
        {
            //_context = context;
            //_mapper = mapper;
            _authorService = authorService;
        }

        // GET: api/authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            try
            {
                var authorDtos = await _authorService.GetAllAuthorsAsync();
                return Ok(authorDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving authors", error = ex.Message });
            }
        }

        // GET: api/authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
        {
            try
            {
                var author = await _authorService.GetAuthorByIdAsync(id);

                if (author == null)
                {
                    return NotFound(new { message = "Author not found" });
                }

                return Ok(author);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the author", error = ex.Message });
            }
        }

        // POST: api/authors
        [HttpPost]
        public async Task<IActionResult> CreateAuthor(CreateAuthorDto createAuthorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = await _authorService.CreateAuthorAsync(createAuthorDto);

                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the author", error = ex.Message });
            }
        }

        // PUT: api/authors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, UpdateAuthorDto updateAuthorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = await _authorService.UpdateAuthorAsync(id, updateAuthorDto);
        
                return NoContent(); // 204 No Content indicates successful update
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the author", error = ex.Message });
            }
        }

        // DELETE: api/authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
               var author = await _authorService.DeleteAuthorAsync(id);

                if (!author)
                {
                    return NotFound(new { message = "Author not found" });
                }
                
                return NoContent(); // 204 No Content indicates successful deletion

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the author", error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> SearchAuthors([FromQuery] string query)
        {
            try
            {
                var authors = await _authorService.SearchAuthorsAsync(query);
                return Ok(authors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching for authors", error = ex.Message });
            }
        }
    }
}