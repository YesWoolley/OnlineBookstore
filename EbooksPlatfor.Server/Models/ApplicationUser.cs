using EbooksPlatform.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string FullName { get; set; } = null!;

        // Navigation properties for relationships
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; }
    }
}
