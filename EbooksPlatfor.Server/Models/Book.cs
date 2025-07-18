using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class Book
    {
        public int Id { get; set; }
       
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        [Url]
        public string? CoverImageUrl { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; } // For inventory tracking

        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<Review>? Reviews { get; set; }
    }
}