
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Models;

namespace Business_Layer.Interfaces;

public interface IPayPalBusiness
{
    Task<OperationResult<PayPalPaymentOrder>> CreateOrder();
    Task<bool> ConfirmPayment(string orderId);
    Task<bool> CancelPayment(string orderId);
    Task<bool> Webhook(JsonDocument body, IHeaderDictionary headers);
}



