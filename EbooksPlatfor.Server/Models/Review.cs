using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public int Rating { get; set; } // 1-5 stars
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}