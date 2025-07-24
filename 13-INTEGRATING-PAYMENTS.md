# Section 13: Integrating PayPal Payments in ASP.NET Core MVC

Welcome to the PayPal payment integration section! In this guide, you’ll learn how to add PayPal checkout to your ASP.NET Core MVC eCommerce app with minimal code, using the PayPal REST API and official SDK.

---

## 🎯 What You’ll Learn

- How to set up a PayPal sandbox for testing
- How to add PayPal settings to your project
- How to create a PayPal payment and redirect users
- How to handle PayPal’s payment confirmation callback
- How to update your order status after payment

---

## 🏗️ Step 1: Set Up PayPal Sandbox

1. Go to [PayPal Developer Dashboard](https://developer.paypal.com/).
2. Create a sandbox business account (for your store) and a personal account (for testing as a buyer).
3. Get your **Client ID** and **Client Secret** from the dashboard.

---

## 🏗️ Step 2: Add PayPal SDK

Install the official PayPal Checkout SDK:

```bash
dotnet add package PayPalCheckoutSdk
```

---

## 🏗️ Step 3: Add PayPal Settings

Add your PayPal credentials to `appsettings.json`:

```json
"PayPal": {
  "ClientId": "YOUR_SANDBOX_CLIENT_ID",
  "ClientSecret": "YOUR_SANDBOX_CLIENT_SECRET",
  "Mode": "sandbox"
}
```

---

## 🏗️ Step 4: Create PayPal Service

Create a service to handle PayPal API calls:

```csharp
// Services/PayPalService.cs
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using BraintreeHttp;
using Microsoft.Extensions.Configuration;

public class PayPalService
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
```

---

## 🏗️ Step 5: Add PayPal to Your Controller

```csharp
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System;

public class PaymentsController : Controller
{
    private readonly PayPalService _paypal;
    public PaymentsController(PayPalService paypal) => _paypal = paypal;

    public async Task<IActionResult> Pay(decimal amount)
    {
        var successUrl = Url.Action("Success", "Payments", null, Request.Scheme) ?? throw new InvalidOperationException("Success URL cannot be null.");
        var cancelUrl = Url.Action("Cancel", "Payments", null, Request.Scheme) ?? throw new InvalidOperationException("Cancel URL cannot be null.");

        var approvalUrl = await _paypal.CreateOrder(amount, successUrl, cancelUrl);
        return Redirect(approvalUrl);
    }

    public async Task<IActionResult> Success(string token)
    {
        var order = await _paypal.CaptureOrder(token);
        // TODO: Update your order/payment status in the database
        return View(order);
    }

    public IActionResult Cancel() => View();
}
```

---

## 🏗️ Step 6: Add PayPal Button to Your View

```html
<!-- In your checkout view (e.g., Checkout.cshtml) -->
<form asp-action="Pay" asp-controller="Payments" method="post">
    <input type="hidden" name="amount" value="@Model.TotalAmount" />
    <button type="submit" class="btn btn-primary">Pay with PayPal</button>
</form>
```

---

## 🧪 Step 7: Test the Integration

1. Run your app and go to the checkout page.
2. Click the “Pay with PayPal” button.
3. Log in with your PayPal sandbox buyer account.
4. Complete the payment and verify you’re redirected back to your site.

---

## 🏆 Best Practices

- Always use the sandbox for testing.
- Never expose your client secret in client-side code.
- Update your order/payment status after successful payment.
- Handle payment failures and cancellations gracefully.

---

**You now have a minimal, course-style PayPal integration in your ASP.NET Core MVC app!**
