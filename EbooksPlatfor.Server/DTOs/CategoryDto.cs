using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends category data TO clients with book count for filtering
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int BookCount { get; set; }
    }

    // Input DTO: Receives category data FROM clients for creation (validation ensures unique names)
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
    }

    // Input DTO: Receives category data FROM clients for updates (validation prevents duplicate names)
    public class UpdateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
    }
}