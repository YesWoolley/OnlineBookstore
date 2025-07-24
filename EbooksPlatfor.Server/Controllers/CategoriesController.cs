using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using AutoMapper;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving categories", error = ex.Message });
            }
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the category", error = ex.Message });
            }
        }

        // GET: api/categories/search?q=searchTerm
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> SearchCategories([FromQuery] string q)
        {
            try
            {
                var categories = await _categoryService.SearchCategoriesAsync(q);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching categories", error = ex.Message });
            }
        }

        // POST: api/categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)  // ← POST: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the category", error = ex.Message });
            }
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)  // ← PUT: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the category", error = ex.Message });
            }
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "Category not found" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the category", error = ex.Message });
            }
        }
    }
}