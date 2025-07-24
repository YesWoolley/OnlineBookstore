using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends clean data TO clients (validation ensures data integrity)
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? CoverImageUrl { get; set; }

        // Related entity names (mapped from relationships)
        public string AuthorName { get; set; } = null!;
        public string PublisherName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;

        // Review summary (calculated properties)
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
    }

    // Detailed response DTO: Sends complete book data TO clients including all reviews
    public class BookDetailDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]
        public int StockQuantity { get; set; }

        [Url(ErrorMessage = "Cover image URL must be a valid URL")]
        public string? CoverImageUrl { get; set; }

        // Related data
        [Required]
        [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters")]
        public string AuthorName { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "Publisher name cannot exceed 100 characters")]
        public string PublisherName { get; set; } = null!;

        [Required]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters")]
        public string CategoryName { get; set; } = null!;

        // Review data (complete review information for detailed views)
        [Range(0, int.MaxValue, ErrorMessage = "Review count cannot be negative")]
        public int ReviewCount { get; set; }

        [Range(0.0, 5.0, ErrorMessage = "Average rating must be between 0 and 5")]
        public double AverageRating { get; set; }

        public ICollection<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    }

    // Input DTO: Receives data FROM clients for creating new books (validation protects API)
    public class CreateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]
        public int StockQuantity { get; set; }

        [Url]
        public string? CoverImageUrl { get; set; }

        // Foreign key IDs (AuthorId is created manually but later mapped to Book.Author.Id via AutoMapper)
        [Required]
        public int AuthorId { get; set; }

        // Foreign key IDs (PublisherId is created manually but later mapped to Book.Publisher.Id via AutoMapper)
        [Required]
        public int PublisherId { get; set; }

        // Foreign key IDs (CategoryId is created manually but later mapped to Book.Category.Id via AutoMapper)
        [Required]
        public int CategoryId { get; set; }
    }

    // Input DTO: Receives data FROM clients for updating existing books (validation ensures data integrity)
    public class UpdateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]
        public int StockQuantity { get; set; }

        [Url]
        public string? CoverImageUrl { get; set; }

        // Foreign key IDs (AuthorId is created manually but later mapped to Book.Author.Id via AutoMapper)
        [Required]
        public int AuthorId { get; set; }

        // Foreign key IDs (PublisherId is created manually but later mapped to Book.Publisher.Id via AutoMapper)
        [Required]
        public int PublisherId { get; set; }

        // Foreign key IDs (CategoryId is created manually but later mapped to Book.Category.Id via AutoMapper)
        [Required]
        public int CategoryId { get; set; }
    }
}