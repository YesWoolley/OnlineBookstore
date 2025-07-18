using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Biography { get; set; }
        public int BookCount { get; set; }
    }

    // Input DTO: Receives author data FROM clients for creation (validation prevents empty names)
    public class CreateAuthorDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(1000)]
        public string? Biography { get; set; }
    }

    // Input DTO: Receives author data FROM clients for updates (validation maintains data quality)
    public class UpdateAuthorDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(1000)]
        public string? Biography { get; set; }
    }
}
