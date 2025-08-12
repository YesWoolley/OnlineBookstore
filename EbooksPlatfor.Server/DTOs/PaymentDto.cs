using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    // Input DTO: Receives payment data FROM clients
    public class CreatePaymentDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "USD";

        public string? Description { get; set; }
    }

    // Response DTO: Sends payment result TO clients
    public class PaymentResultDto
    {
        public bool Success { get; set; }
        public string? OrderId { get; set; }
        public string? ApprovalUrl { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
    }

    // Response DTO: Sends payment capture result TO clients
    public class PaymentCaptureDto
    {
        public bool Success { get; set; }
        public string? OrderId { get; set; }
        public string? Status { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? Message { get; set; }
    }
} 