using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Enums;
using Presentation_Layer.Authorization;
using Business_Layer.Interfaces;




namespace Presentation_Layer.Controllers;

[ApiController]
[Route("[controller]")]
public class PayPalController : ControllerBase
{
    public ILogger<PayPalController> logger { get; }
    public IPayPalBusiness PayPalBusiness { get; }

    public PayPalController(ILogger<PayPalController> logger, IPayPalBusiness payPalBusiness)
    {
        this.logger = logger;
        PayPalBusiness = payPalBusiness;
    }



    [Authorize]
    [CheckPermission(Permission.Paypal_MakePayPalPayment)]
    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder()
    {
        var result = await PayPalBusiness.CreateOrder();

        if (result.Success)
        {
            return Ok(result.Data);
        }

        return BadRequest(result.ErrorMessage);
    }



    [Authorize]
    [CheckPermission(Permission.Paypal_MakePayPalPayment)]
    [HttpGet("confirm-payment")]
    public async Task<IActionResult> ConfirmPayment([FromQuery] string token)
    {
        return await PayPalBusiness.ConfirmPayment(token) ?
            Ok() : BadRequest();
    }



    [Authorize]
    [CheckPermission(Permission.Paypal_MakePayPalPayment)]
    [HttpGet("payment-cancelled")]
    public async Task<IActionResult> CancelPayment([FromQuery] string token)
    {
        return await PayPalBusiness.CancelPayment(token) ?
            Ok() : BadRequest();
    }



    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> ReceiveWebhook()
    {
        using var reader = new StreamReader(Request.Body);
        var body = JsonDocument.Parse(await reader.ReadToEndAsync());
        var headers = HttpContext.Request.Headers;

        if (!await PayPalBusiness.Webhook(body, headers))
        {
            return BadRequest();
        }

        return Ok();
    }

}

