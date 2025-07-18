using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using OnlineBookstore.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OnlineBookstore.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AuthorService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
        {
            var authors = await _context.Authors
                .Include(a => a.Books)
                .ToListAsync();

            var authorDtos = _mapper.Map<IEnumerable<AuthorDto>>(authors);
            return authorDtos;
        }

        public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            return author != null ? _mapper.Map<AuthorDto>(author) : null;
        }

        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
        {
            var author = _mapper.Map<Author>(createAuthorDto);

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            // For new authors, BookCount will be 0 (no books yet)
            var authorDto = _mapper.Map<AuthorDto>(author);
            authorDto.BookCount = 0; // Explicitly set for new authors

            return authorDto;
        }

        public async Task<AuthorDto> UpdateAuthorAsync(int id, UpdateAuthorDto updateAuthorDto)
        {
            var author = await _context.Authors.FindAsync(id) ?? throw new ArgumentException("Author not found");
            _mapper.Map(updateAuthorDto, author);
            await _context.SaveChangesAsync();

            return _mapper.Map<AuthorDto>(author);
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return false;
            }

            if (author.Books != null && author.Books.Any())
            {
                throw new InvalidOperationException("Cannot delete author with existing books");
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<AuthorDto>> SearchAuthorsAsync(string searchTerm)
        {
            // Business logic: Empty search = return all authors
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAuthorsAsync(); // Returns all authors
            }

            var authors = await _context.Authors
                .Include(a => a.Books)
                .Where(a => a.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return _mapper.Map<IEnumerable<AuthorDto>>(authors);
        }
    }
}
