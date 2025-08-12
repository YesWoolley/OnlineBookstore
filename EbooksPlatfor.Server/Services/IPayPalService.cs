using PayPalCheckoutSdk.Orders;

namespace OnlineBookstore.Services
{
    public interface IPayPalService
    {
        Task<string> CreateOrder(decimal amount, string returnUrl, string cancelUrl);
        Task<Order> CaptureOrder(string orderId);
    }
} 