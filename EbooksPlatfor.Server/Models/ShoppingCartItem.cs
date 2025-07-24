using OnlineBookstore.Models;
using System.ComponentModel.DataAnnotations;

namespace EbooksPlatform.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        // Navigation properties for relationships
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}
