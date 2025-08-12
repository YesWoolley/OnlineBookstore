using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;

namespace OnlineBookstore.Services
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BookService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);

            return book != null ? _mapper.Map<BookDto>(book) : null;
        }

        public async Task<BookDetailDto?> GetBookDetailByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews!)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            return book != null ? _mapper.Map<BookDetailDto>(book) : null;
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
        {
            // Validate that related entities exist
            var author = await _context.Authors.FindAsync(createBookDto.AuthorId);
            if (author == null)
                throw new ArgumentException("Author not found");

            var publisher = await _context.Publishers.FindAsync(createBookDto.PublisherId);
            if (publisher == null)
                throw new ArgumentException("Publisher not found");

            var category = await _context.Categories.FindAsync(createBookDto.CategoryId);
            if (category == null)
                throw new ArgumentException("Category not found");

            var book = _mapper.Map<Book>(createBookDto);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Reload with related data for response
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

            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> UpdateBookAsync(int id, UpdateBookDto updateBookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                throw new ArgumentException("Book not found");

            // Validate that related entities exist
            var author = await _context.Authors.FindAsync(updateBookDto.AuthorId);
            if (author == null)
                throw new ArgumentException("Author not found");

            var publisher = await _context.Publishers.FindAsync(updateBookDto.PublisherId);
            if (publisher == null)
                throw new ArgumentException("Publisher not found");

            var category = await _context.Categories.FindAsync(updateBookDto.CategoryId);
            if (category == null)
                throw new ArgumentException("Category not found");

            _mapper.Map(updateBookDto, book);

            await _context.SaveChangesAsync();

            return _mapper.Map<BookDto>(book);
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return false;
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
        {
            try
            {
                Console.WriteLine($"SearchBooksAsync called with searchTerm: '{searchTerm}'");
                
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    Console.WriteLine("Search term is null or empty, returning all books");
                    return await GetAllBooksAsync();
                }

                Console.WriteLine("Starting database query...");
                var books = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Publisher)
                    .Include(b => b.Category)
                    .Include(b => b.Reviews)
                    .Where(b => EF.Functions.Like(b.Title, $"%{searchTerm}%") ||
                               (b.Author != null && EF.Functions.Like(b.Author.Name, $"%{searchTerm}%")) ||
                               (b.Category != null && EF.Functions.Like(b.Category.Name, $"%{searchTerm}%")) ||
                               (b.Publisher != null && EF.Functions.Like(b.Publisher.Name, $"%{searchTerm}%")))
                    .ToListAsync();

                Console.WriteLine($"Database query completed, found {books.Count} books");
                var result = _mapper.Map<IEnumerable<BookDto>>(books);
                Console.WriteLine($"Mapping completed, returning {result.Count()} DTOs");
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SearchBooksAsync error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(int categoryId)
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .Where(b => b.CategoryId == categoryId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(int authorId)
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .Where(b => b.AuthorId == authorId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByPublisherAsync(int publisherId)
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .Where(b => b.PublisherId == publisherId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
    }
}