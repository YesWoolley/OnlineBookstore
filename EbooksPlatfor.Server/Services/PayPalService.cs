using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using OnlineBookstore.Services;

namespace OnlineBookstore.Services
{
    public class PayPalService : IPayPalService
{
    private readonly IConfiguration _config;
    public PayPalService(IConfiguration config) => _config = config;

    private PayPalEnvironment GetEnvironment() =>
        _config["PayPal:Mode"] == "live"
            ? new LiveEnvironment(_config["PayPal:ClientId"], _config["PayPal:ClientSecret"])
            : new SandboxEnvironment(_config["PayPal:ClientId"], _config["PayPal:ClientSecret"]);

    public PayPalHttpClient GetClient() => new PayPalHttpClient(GetEnvironment());

    public async Task<string> CreateOrder(decimal amount, string returnUrl, string cancelUrl)
    {
        var orderRequest = new OrderRequest()
        {
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest { AmountWithBreakdown = new AmountWithBreakdown { CurrencyCode = "USD", Value = amount.ToString("F2") } }
            },
            ApplicationContext = new ApplicationContext
            {
                ReturnUrl = returnUrl,
                CancelUrl = cancelUrl
            }
        };

        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(orderRequest);

        var response = await GetClient().Execute(request);
        var result = response.Result<Order>();
        return result.Links.First(x => x.Rel == "approve").Href; // PayPal approval URL
    }

    public async Task<Order> CaptureOrder(string orderId)
    {
        var request = new OrdersCaptureRequest(orderId);
        request.RequestBody(new OrderActionRequest());
        var response = await GetClient().Execute(request);
        return response.Result<Order>();
    }
}
}