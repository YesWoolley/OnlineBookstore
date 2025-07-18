using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;

namespace OnlineBookstore.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PublisherService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PublisherDto>> GetAllPublishersAsync()
        {
            var publishers = await _context.Publishers
                .Include(p => p.Books)
                .ToListAsync();

            var publisherDtos = _mapper.Map<IEnumerable<PublisherDto>>(publishers);
            return publisherDtos;
        }

        public async Task<PublisherDto?> GetPublisherByIdAsync(int id)
        {
            var publisher = await _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            return publisher != null ? _mapper.Map<PublisherDto>(publisher) : null;
        }

        public async Task<PublisherDto> CreatePublisherAsync(CreatePublisherDto createPublisherDto)
        {
            var publisher = _mapper.Map<Publisher>(createPublisherDto);

            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            // For new authors, BookCount will be 0 (no books yet)
            var publisherDto = _mapper.Map<PublisherDto>(publisher);
            publisherDto.BookCount = 0; // Explicitly set for new authors

            return publisherDto;
        }

        public async Task<PublisherDto> UpdatePublisherAsync(int id, UpdatePublisherDto updatePublisherDto)
        {
            var publisher = await _context.Publishers.FindAsync(id) ?? throw new ArgumentException("Publisher not found");
            
            _mapper.Map(updatePublisherDto, publisher);

            await _context.SaveChangesAsync();

            return _mapper.Map<PublisherDto>(publisher);
        }

        public async Task<bool> DeletePublisherAsync(int id)
        {
            var publisher = await _context.Publishers
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (publisher == null)
            {
                return false;
            }

            if (publisher.Books != null && publisher.Books.Any())
            {
                throw new InvalidOperationException("Cannot delete author with existing books");
            }

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PublisherDto>> SearchPublishersAsync(string searchTerm)
        {
            // Business logic: Empty search = return all authors
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllPublishersAsync(); // Returns all authors
            }

            var publishers = await _context.Publishers
                .Include(p => p.Books)
                .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return _mapper.Map<IEnumerable<PublisherDto>>(publishers);
        }
    }
}
