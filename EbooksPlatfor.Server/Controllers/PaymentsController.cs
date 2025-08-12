using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Services;
using System;

namespace OnlineBookstore.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IPayPalService _paypal;
        public PaymentsController(IPayPalService paypal) => _paypal = paypal;

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
}