using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;

namespace OnlineBookstore.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoryService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Books)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Books)
                .FirstOrDefaultAsync(c => c.Id == id);

            return category != null ? _mapper.Map<CategoryDto>(category) : null;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // For new categories, BookCount will be 0 (no books yet)
            var categoryDto = _mapper.Map<CategoryDto>(category);
            categoryDto.BookCount = 0; // Explicitly set for new categories

            return categoryDto;
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _context.Categories.FindAsync(id) ?? throw new ArgumentException("Category not found");


            _mapper.Map(updateCategoryDto, category);

            await _context.SaveChangesAsync();

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Books)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return false;
            }

            if (category.Books != null && category.Books.Any())
            {
                throw new InvalidOperationException("Cannot delete category with existing books");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CategoryDto>> SearchCategoriesAsync(string searchTerm)
        {
            // Business logic: Empty search = return all categories
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllCategoriesAsync(); // Returns all categories
            }

            var categories = await _context.Categories
                .Include(c => c.Books)
                .Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
    }
}
