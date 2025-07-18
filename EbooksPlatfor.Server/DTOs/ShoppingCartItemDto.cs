using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends cart item data TO clients with book details for display
    public class ShoppingCartItemDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public decimal BookPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // Input DTO: Receives cart item data FROM clients for creation (validation ensures valid quantities)
    public class CreateShoppingCartItemDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    // Input DTO: Receives cart item data FROM clients for updates (validation maintains cart integrity)
    public class UpdateShoppingCartItemDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}