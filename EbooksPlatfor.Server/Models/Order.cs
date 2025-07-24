using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string OrderStatus { get; set; } = null!; // Pending, Shipped, Delivered, etc.
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
