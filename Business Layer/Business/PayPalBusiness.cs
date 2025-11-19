using Enums;
using Options;
using Business_Layer.Payment;
using PayPalCheckoutSdk.Orders;
using System.Globalization;
using System.Text.Json;
using Order = Models.Order;
using PaymentOrder = PayPalCheckoutSdk.Orders.Order;
using JsonSerializer = System.Text.Json.JsonSerializer;
using PayoutsSdk.Payouts;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Data_Layer.Interfaces;
using Business_Layer.Interfaces;
using Models;
using Microsoft.AspNetCore.Http;

namespace Business_Layer.Business;

public class PayPalBusiness : IPayPalBusiness
{
    public PaypalOptions PaypalOptions { get; }
    public PaypalUrls PaypalUrls { get; }
    public IPayPalData PayPalData { get; }
    public IOrdersBusiness OrdersBusiness { get; }
    public ILogger<PayPalBusiness> Logger { get; }
    public ISalesBusiness SalesBusiness { get; }
    public ICartItemBusiness CartItemBusiness { get; }
    public IUsersBusiness UsersBusiness { get; }

    public PayPalBusiness
        (
        PaypalOptions paypalOptions,
        PaypalUrls paypalUrls,
        IPayPalData payPalData,
        IOrdersBusiness ordersBusiness,
        ILogger<PayPalBusiness> logger,
        ISalesBusiness salesBusiness,
        ICartItemBusiness cartItemBusiness,
        IUsersBusiness usersBusiness

        )
    {
        PaypalOptions = paypalOptions;
        PaypalUrls = paypalUrls;
        PayPalData = payPalData;
        OrdersBusiness = ordersBusiness;
        Logger = logger;
        SalesBusiness = salesBusiness;
        CartItemBusiness = cartItemBusiness;
        UsersBusiness = usersBusiness;
    }




    public async Task<OperationResult<PayPalPaymentOrder>> CreateOrder()
    {
        OperationResult<PayPalPaymentOrder> operationResult = new();


        Order order = await OrdersBusiness.CreateOrder();

        if (order == null)
        {
            if (await CartItemBusiness.SyncCartItemsPromocode())
            {
                operationResult.ErrorMessage = "The quantity of products has been modified to match the quantity of the promo codes.";
            }
            else
            {
                operationResult.ErrorMessage = "Something went Wrong";
            }

            return operationResult;
        }

        if (!await OrdersBusiness.CreateStoreOrder(order.Id))
        {
            if (await CartItemBusiness.SyncCartItemsWithStocks())
            {
                operationResult.ErrorMessage = "The quantity of products has been modified to match the quantity of the stocks.";
            }
            else
            {
                operationResult.ErrorMessage = "Something went Wrong";
            }

            return operationResult;
        }

        try
        {
            var paymentRequest = await PaymentResponse(order.TotalPrice);

            if (await PayPalData.SaveOrderPayment(paymentRequest.PaymentId, order.Id))
            {
                operationResult.IsSuccess = true;
                operationResult.Data = paymentRequest;
                return operationResult;
            }

            operationResult.ErrorMessage = "Something went wrong";
        }
        catch (Exception ex)
        {
            operationResult.ErrorMessage = "Something went wrong";
        }

        return operationResult;
    }
    public async Task<bool> ConfirmPayment(string paymentId)
    {
        PaymentOrder paymentOrder = null;

        if (!await VerifyPaymentAccess(paymentId))
        {
            return false;
        }

        try
        {
            await GetPaymentOrder(paymentId);
        }
        catch (Exception)
        {
            return false;
        }

        if (paymentOrder != null && paymentOrder.Status == "COMPLETED")
        {
            return await PayPalData.UpdatePaymentStateId(paymentId, (int)PaymentState.Approved);
        }

        return false;
    }
    public async Task<bool> CancelPayment(string paymentId)
    {
        if (!await VerifyPaymentAccess(paymentId))
        {
            return false;
        }

        return await UpdatePaymentStateId(paymentId, PaymentState.Cancelled);
    }
    public async Task<bool> Webhook(JsonDocument body, IHeaderDictionary headers)
    {
        if (!await VerifyEvent(body, headers))
        {
            return false;
        }

        if (body.RootElement.TryGetProperty("event_type", out var eventType))
        {
            string eventName = eventType.GetString();

            Logger.LogCritical("Webhook Start With Event:{event}", eventName);

            if (eventName == "PAYMENT.CAPTURE.COMPLETED")
            {
                var resource = body.RootElement.GetProperty("resource");
                var supplementaryData = resource.GetProperty("supplementary_data");
                var relatedIds = supplementaryData.GetProperty("related_ids");
                string paymentId = relatedIds.GetProperty("order_id").ToString();

                var paymentDetails = await GetPaymentDetails(paymentId);

                if (!await UpdatePaymentStateId(paymentId, PaymentState.Completed))
                {
                    Logger.LogCritical("Update payment state to completed failed, PaymentId {id}", paymentId);
                }
                if (!await OrdersBusiness.ConfrimOrderInStore(paymentDetails.orderId))
                {
                    Logger.LogCritical("Confirm Order in store failed, OrderId {id}", paymentDetails.orderId);
                }
            }
        }

        return true;
    }
    public async Task PayForSellers()
    {
        var SellersAccounting = await SalesBusiness.GetNewMerchantAccountingDetails();

        foreach (var sellerAccounting in SellersAccounting)
        {
            await CreatePayout(sellerAccounting);
        }
    }


    private async Task<bool> UpdatePaymentStateId(string paymentId, PaymentState state)
    {
        if (!Enum.IsDefined(typeof(PaymentState), state))
        {
            return false;
        }
        return await PayPalData.UpdatePaymentStateId(paymentId, (int)state);
    }

    private async Task<PaymentDetails> GetPaymentDetails(string paymentId)
    {
        return await PayPalData.GetPaymentDetails(paymentId);
    }

    private async Task<PaymentOrder> GetPaymentOrder(string paymentId)
    {
        var request = new OrdersCaptureRequest(paymentId);
        request.RequestBody(new OrderActionRequest());

        var client = PayPalClient.Client(PaypalOptions.ClientId, PaypalOptions.ClientSecret);
        var response = await client.Execute(request);
        return response.Result<PaymentOrder>();
    }

    private async Task<bool> VerifyPaymentAccess(string paymentId)
    {
        var paymentDetails = await GetPaymentDetails(paymentId);

        if (paymentDetails == null)
        {
            return false;
        }
        if (paymentDetails.userId != UsersBusiness.GetUserId())
        {
            return false;
        }
        if (paymentDetails.stateId != (int)PaymentState.New)
        {
            return false;
        }

        return true;
    }

    private async Task<bool> VerifyEvent(JsonDocument body, IHeaderDictionary headers)
    {
        var verifyRequest = new object();

        try
        {
            verifyRequest = new
            {
                auth_algo = headers["Paypal-Auth-Algo"].FirstOrDefault(),
                cert_url = headers["Paypal-Cert-Url"].FirstOrDefault(),
                transmission_id = headers["Paypal-Transmission-Id"].FirstOrDefault(),
                transmission_sig = headers["Paypal-Transmission-Sig"].FirstOrDefault(),
                transmission_time = headers["Paypal-Transmission-Time"].FirstOrDefault(),
                webhook_event = body.RootElement,
                webhook_id = PaypalOptions.WebhookId
            };

        }
        catch (Exception)
        {
            return false;
        }

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(verifyRequest),
            Encoding.UTF8,
            "application/json");

        using var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken());

        var response = await client.PostAsync(
            "https://api.sandbox.paypal.com/v1/notifications/verify-webhook-signature",
            jsonContent);


        if (!response.IsSuccessStatusCode)
            return false;

        var responseString = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseString);
        var status = doc.RootElement.GetProperty("verification_status").GetString();

        return status == "SUCCESS";
    }

    private async Task<string> GetAccessToken()
    {
        var client = new HttpClient();
        var byteArray = Encoding.ASCII.GetBytes($"{PaypalOptions.ClientId}:{PaypalOptions.ClientSecret}");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var postData = new Dictionary<string, string>
                    {
                        { "grant_type", "client_credentials" }
                    };

        var response = await client.PostAsync("https://api.sandbox.paypal.com/v1/oauth2/token", new FormUrlEncodedContent(postData));
        string content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        return json.RootElement.GetProperty("access_token").GetString();
    }

    private async Task CreatePayout(MerchantAccountingDetails sellerAccounting)
    {
        try
        {
            var PayoutBody = CreatePaypalPayoutBody(sellerAccounting);
            var result = await PayoutResponse(PayoutBody);

            if (result.BatchHeader.BatchStatus == "SUCCESS")
            {
                await SalesBusiness.UpdateMerchantAccountState(sellerAccounting.id, MerchantAccountingState.Paid);
                await PayPalData.SaveTransferPayout(result.BatchHeader.PayoutBatchId, sellerAccounting.id);
            }
            else
            {
                await SalesBusiness.UpdateMerchantAccountState(sellerAccounting.id, MerchantAccountingState.Pending);
            }
        }
        catch (Exception ex)
        {
            await SalesBusiness.UpdateMerchantAccountState(sellerAccounting.id, MerchantAccountingState.Pending);
        }
    }

    private CreatePayoutRequest CreatePaypalPayoutBody(MerchantAccountingDetails sellerAccounting)
    {
        string emailSubject = @$"Dear {sellerAccounting.sellerName},{sellerAccounting.priceAfterTax} has been successfully transferred to your account";
        return new CreatePayoutRequest()
        {
            SenderBatchHeader = new SenderBatchHeader()
            {
                EmailSubject = emailSubject,
                SenderBatchId = Guid.NewGuid().ToString()
            },
            Items = new List<PayoutItem>()
        {
            new PayoutItem()
            {
                RecipientType = "EMAIL",
                Receiver = sellerAccounting.email,
                Amount = new Currency()
                {
                    CurrencyCode = "USD",
                    Value = sellerAccounting.priceAfterTax.ToString(CultureInfo.InvariantCulture)
                }
            }
        }
        };
    }

    private async Task<CreatePayoutResponse> PayoutResponse(CreatePayoutRequest body)
    {
        var client = PayPalClient.Client(PaypalOptions.ClientId, PaypalOptions.ClientSecret);
        var request = new PayoutsPostRequest();
        request.RequestBody(body);
        var response = await client.Execute(request);
        return response.Result<CreatePayoutResponse>();
    }

    private OrderRequest CreatePaypalPaymentBody(decimal price)
    {
        return new OrderRequest()
        {
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = "USD",
                        Value = price.ToString(CultureInfo.InvariantCulture)
                    }
                }
            },
            ApplicationContext = new ApplicationContext
            {
                ReturnUrl = PaypalUrls.Confirm,
                CancelUrl = PaypalUrls.Cancel
            }
        };
    }

    private async Task<PayPalPaymentOrder> PaymentResponse(decimal price)
    {
        var client = PayPalClient.Client(PaypalOptions.ClientId, PaypalOptions.ClientSecret);

        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(CreatePaypalPaymentBody(price));

        var response = await client.Execute(request);
        var result = response.Result<PaymentOrder>();
        var approvalLink = result.Links.Find(x => x.Rel == "approve")?.Href;

        return new PayPalPaymentOrder
        {
            PaymentId = result.Id,
            ApprovalLink = approvalLink
        };
    }
}



