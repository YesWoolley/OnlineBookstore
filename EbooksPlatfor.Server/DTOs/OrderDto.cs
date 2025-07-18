using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Response DTO: Sends order data TO clients with user name and order items for display
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    // Input DTO: Receives order data FROM clients for creation (validation ensures valid order)
    public class CreateOrderDto
    {
        [Required]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        public string OrderStatus { get; set; } = null!;

        [Required]
        public ICollection<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }

    // Input DTO: Receives order data FROM clients for updates (validation maintains order integrity)
    public class UpdateOrderDto
    {
        [Required]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        public string OrderStatus { get; set; } = null!;
    }
}