using EbooksPlatform.Models;
using Microsoft.AspNetCore.Identity;

namespace OnlineBookstore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        // Navigation properties for relationships
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; }
    }
}
