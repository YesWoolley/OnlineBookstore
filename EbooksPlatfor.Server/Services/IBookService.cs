using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDetailDto?> GetBookDetailByIdAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookDto createBookDto);
        Task<BookDto> UpdateBookAsync(int id, UpdateBookDto updateBookDto);
        Task<bool> DeleteBookAsync(int id);
        Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);
        Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(int categoryId);
        Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(int authorId);
        Task<IEnumerable<BookDto>> GetBooksByPublisherAsync(int publisherId);
    }
}