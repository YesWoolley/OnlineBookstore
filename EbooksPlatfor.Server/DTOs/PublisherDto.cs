using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends publisher data TO clients with book count for display
    public class PublisherDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int BookCount { get; set; }
    }

    // Input DTO: Receives publisher data FROM clients for creation (validation prevents empty names)
    public class CreatePublisherDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }
    }

    // Input DTO: Receives publisher data FROM clients for updates (validation maintains data quality)
    public class UpdatePublisherDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}