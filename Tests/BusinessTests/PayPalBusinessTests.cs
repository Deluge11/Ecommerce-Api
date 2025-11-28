using Business_Layer.Business;
using Business_Layer.Interfaces;
using Data_Layer.Interfaces;
using Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models;
using Moq;
using Moq.Protected;
using Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PaymentOrder = PayPalCheckoutSdk.Orders.Order;

namespace Tests.BusinessTests
{
    public class PayPalBusinessTests
    {

        [Fact]
        public async Task CreateOrder_ShouldReturnNotSucces_WhenCreateStocksOrderNotSuccess()
        {
            var ordersBusinessMock = new Mock<IOrdersBusiness>();
            var payPalDataMock = new Mock<IPayPalData>();

            var createStockOrderOperation = new OperationResult<Order>();
            ordersBusinessMock.Setup(o => o.CreateOrder()).ReturnsAsync(createStockOrderOperation);
            payPalDataMock.Setup(p => p.SaveOrderPayment(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(false);

            var payPalBusinessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                payPalDataMock.Object,
                ordersBusinessMock.Object,
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            {
                CallBase = true
            };

            payPalBusinessMock.Protected()
            .Setup<Task<PayPalPaymentOrder>>(
                "CreatePaymentOrder",
                ItExpr.IsAny<decimal>()
            )
            .ReturnsAsync(new PayPalPaymentOrder
            {
                PaymentId = "PAY-123",
                ApprovalLink = "http://fake-approval-link"
            });


            var result = await payPalBusinessMock.Object.CreateOrder();


            Assert.False(result.Success);
            Assert.Null(result.Data);
            ordersBusinessMock.Verify(o => o.CreateOrder(), Times.Once);
            payPalDataMock.Verify(p => p.SaveOrderPayment(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            payPalBusinessMock.Protected().Verify("CreatePaymentOrder", Times.Never(), ItExpr.IsAny<decimal>());
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnNotSucces_WhenCreateStocksOrderReturnDataEqualNull()
        {
            var ordersBusinessMock = new Mock<IOrdersBusiness>();
            var payPalDataMock = new Mock<IPayPalData>();

            var createStockOrderOperation = new OperationResult<Order>();
            createStockOrderOperation.Success = true;
            ordersBusinessMock.Setup(o => o.CreateOrder()).ReturnsAsync(createStockOrderOperation);
            payPalDataMock.Setup(p => p.SaveOrderPayment(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(false);

            var payPalBusinessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                payPalDataMock.Object,
                ordersBusinessMock.Object,
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            {
                CallBase = true
            };

            payPalBusinessMock.Protected()
            .Setup<Task<PayPalPaymentOrder>>(
                "CreatePaymentOrder",
                ItExpr.IsAny<decimal>()
            )
            .ReturnsAsync(new PayPalPaymentOrder
            {
                PaymentId = "PAY-123",
                ApprovalLink = "http://fake-approval-link"
            });


            var result = await payPalBusinessMock.Object.CreateOrder();


            Assert.False(result.Success);
            Assert.Null(result.Data);
            ordersBusinessMock.Verify(o => o.CreateOrder(), Times.Once);
            payPalBusinessMock.Protected().Verify("CreatePaymentOrder", Times.Never(), ItExpr.IsAny<decimal>());
            payPalDataMock.Verify(p => p.SaveOrderPayment(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnNotSucces_WhenCreatePaymentOrderReturnNull()
        {
            var ordersBusinessMock = new Mock<IOrdersBusiness>();
            var payPalDataMock = new Mock<IPayPalData>();

            var createStockOrderOperation = new OperationResult<Order>();
            createStockOrderOperation.Success = true;
            createStockOrderOperation.Data = new Order();

            ordersBusinessMock.Setup(o => o.CreateOrder()).ReturnsAsync(createStockOrderOperation);
            payPalDataMock.Setup(p => p.SaveOrderPayment(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(false);

            var payPalBusinessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                payPalDataMock.Object,
                ordersBusinessMock.Object,
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            {
                CallBase = true
            };

            payPalBusinessMock.Protected()
            .Setup<Task<PayPalPaymentOrder>>("CreatePaymentOrder", ItExpr.IsAny<decimal>())
            .Returns(Task.FromResult<PayPalPaymentOrder>(null!));

            var result = await payPalBusinessMock.Object.CreateOrder();


            Assert.False(result.Success);
            Assert.Null(result.Data);
            ordersBusinessMock.Verify(o => o.CreateOrder(), Times.Once);
            payPalBusinessMock.Protected().Verify("CreatePaymentOrder", Times.Once(), ItExpr.IsAny<decimal>());
            payPalDataMock.Verify(p => p.SaveOrderPayment(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnNotSucces_WhenSavePaymentOrderFailed()
        {
            var ordersBusinessMock = new Mock<IOrdersBusiness>();
            var payPalDataMock = new Mock<IPayPalData>();

            var createStockOrderOperation = new OperationResult<Order>();
            createStockOrderOperation.Success = true;
            createStockOrderOperation.Data = new Order();

            ordersBusinessMock.Setup(o => o.CreateOrder()).ReturnsAsync(createStockOrderOperation);
            payPalDataMock.Setup(p => p.SaveOrderPayment(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(false);

            var payPalBusinessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                payPalDataMock.Object,
                ordersBusinessMock.Object,
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            {
                CallBase = true
            };

            payPalBusinessMock.Protected()
            .Setup<Task<PayPalPaymentOrder>>("CreatePaymentOrder", ItExpr.IsAny<decimal>())
            .ReturnsAsync(new PayPalPaymentOrder());

            var result = await payPalBusinessMock.Object.CreateOrder();


            Assert.False(result.Success);
            Assert.Null(result.Data);
            ordersBusinessMock.Verify(o => o.CreateOrder(), Times.Once);
            payPalBusinessMock.Protected().Verify("CreatePaymentOrder", Times.Once(), ItExpr.IsAny<decimal>());
            payPalDataMock.Verify(p => p.SaveOrderPayment(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnNSuccess_WhenAllValid()
        {
            var ordersBusinessMock = new Mock<IOrdersBusiness>();
            var payPalDataMock = new Mock<IPayPalData>();

            var createStockOrderOperation = new OperationResult<Order>();
            createStockOrderOperation.Success = true;
            createStockOrderOperation.Data = new Order();

            ordersBusinessMock.Setup(o => o.CreateOrder()).ReturnsAsync(createStockOrderOperation);
            payPalDataMock.Setup(p => p.SaveOrderPayment(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);

            var payPalBusinessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                payPalDataMock.Object,
                ordersBusinessMock.Object,
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            {
                CallBase = true
            };

            payPalBusinessMock.Protected()
            .Setup<Task<PayPalPaymentOrder>>("CreatePaymentOrder", ItExpr.IsAny<decimal>())
            .ReturnsAsync(new PayPalPaymentOrder());

            var result = await payPalBusinessMock.Object.CreateOrder();


            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            ordersBusinessMock.Verify(o => o.CreateOrder(), Times.Once);
            payPalBusinessMock.Protected().Verify("CreatePaymentOrder", Times.Once(), ItExpr.IsAny<decimal>());
            payPalDataMock.Verify(p => p.SaveOrderPayment(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ConfirmPayment_ShouldReturnFalse_WhenVerifyPaymentAccessFails()
        {
            var payPalDataMock = new Mock<IPayPalData>();
            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                payPalDataMock.Object,
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaymentAccess", ItExpr.IsAny<string>())
                .ReturnsAsync(false);

            var result = await businessMock.Object.ConfirmPayment("PAY-123");

            Assert.False(result);
            businessMock.Protected().Verify("VerifyPaymentAccess", Times.Once(), ItExpr.IsAny<string>());
            businessMock.Protected().Verify("GetPaymentOrder", Times.Never(), ItExpr.IsAny<string>());
            payPalDataMock.Verify(p => p.UpdatePaymentStateId(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ConfirmPayment_ShouldReturnFalse_WhenPaymentOrderIsNull()
        {
            var payPalDataMock = new Mock<IPayPalData>();
            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                payPalDataMock.Object,
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaymentAccess", ItExpr.IsAny<string>())
                .ReturnsAsync(true);

            businessMock.Protected()
                .Setup<Task<PaymentOrder>>("GetPaymentOrder", ItExpr.IsAny<string>())
                .ReturnsAsync((PaymentOrder)null!);

            var result = await businessMock.Object.ConfirmPayment("PAY-123");

            Assert.False(result);
            businessMock.Protected().Verify("GetPaymentOrder", Times.Once(), ItExpr.IsAny<string>());
            payPalDataMock.Verify(p => p.UpdatePaymentStateId(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ConfirmPayment_ShouldReturnFalse_WhenPaymentStatusIsNotCompleted()
        {
            var payPalDataMock = new Mock<IPayPalData>();
            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                payPalDataMock.Object,
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaymentAccess", ItExpr.IsAny<string>())
                .ReturnsAsync(true);

            businessMock.Protected()
                .Setup<Task<PaymentOrder>>("GetPaymentOrder", ItExpr.IsAny<string>())
                .ReturnsAsync(new PaymentOrder { Status = "PENDING" });

            var result = await businessMock.Object.ConfirmPayment("PAY-123");

            Assert.False(result);
            businessMock.Protected().Verify("GetPaymentOrder", Times.Once(), ItExpr.IsAny<string>());
            payPalDataMock.Verify(p => p.UpdatePaymentStateId(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ConfirmPayment_ShouldReturnTrue_WhenPaymentCompletedAndSaved()
        {
            var payPalDataMock = new Mock<IPayPalData>();
            payPalDataMock.Setup(p => p.UpdatePaymentStateId(It.IsAny<string>(), It.IsAny<int>()))
                          .ReturnsAsync(true);

            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                payPalDataMock.Object,
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaymentAccess", ItExpr.IsAny<string>())
                .ReturnsAsync(true);

            businessMock.Protected()
                .Setup<Task<PaymentOrder>>("GetPaymentOrder", ItExpr.IsAny<string>())
                .ReturnsAsync(new PaymentOrder { Status = "COMPLETED" });

            var result = await businessMock.Object.ConfirmPayment("PAY-123");

            Assert.True(result);
            payPalDataMock.Verify(p => p.UpdatePaymentStateId("PAY-123", (int)PaymentState.Approved), Times.Once);
            businessMock.Protected().Verify("GetPaymentOrder", Times.Once(), ItExpr.IsAny<string>());
            payPalDataMock.Verify(p => p.UpdatePaymentStateId(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ConfirmPayment_ShouldReturnFalse_WhenUpdatePaymentStateIdFails()
        {
            var payPalDataMock = new Mock<IPayPalData>();
            payPalDataMock.Setup(p => p.UpdatePaymentStateId(It.IsAny<string>(), It.IsAny<int>()))
                          .ReturnsAsync(false);

            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                payPalDataMock.Object,
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaymentAccess", ItExpr.IsAny<string>())
                .ReturnsAsync(true);

            businessMock.Protected()
                .Setup<Task<PaymentOrder>>("GetPaymentOrder", ItExpr.IsAny<string>())
                .ReturnsAsync(new PaymentOrder { Status = "COMPLETED" });

            var result = await businessMock.Object.ConfirmPayment("PAY-123");

            Assert.False(result);
            payPalDataMock.Verify(p => p.UpdatePaymentStateId("PAY-123", (int)PaymentState.Approved), Times.Once);
            payPalDataMock.Verify(p => p.UpdatePaymentStateId("PAY-123", (int)PaymentState.Approved), Times.Once);
            businessMock.Protected().Verify("GetPaymentOrder", Times.Once(), ItExpr.IsAny<string>());
            payPalDataMock.Verify(p => p.UpdatePaymentStateId(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task CancelPayment_ShouldReturnFalse_WhenVerifyPaymentAccessFails()
        {
            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                Mock.Of<IPayPalData>(),
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaymentAccess", ItExpr.IsAny<string>())
                .ReturnsAsync(false);

            var result = await businessMock.Object.CancelPayment("PAY-123");

            Assert.False(result);
            businessMock.Protected().Verify("VerifyPaymentAccess", Times.Once(), ItExpr.IsAny<string>());
            businessMock.Protected().Verify("UpdatePaymentStateId", Times.Never(), ItExpr.IsAny<string>(), ItExpr.IsAny<PaymentState>());
        }

        [Fact]
        public async Task CancelPayment_ShouldReturnFalse_WhenUpdatePaymentStateIdFails()
        {
            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                Mock.Of<IPayPalData>(),
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaymentAccess", ItExpr.IsAny<string>())
                .ReturnsAsync(true);

            businessMock.Protected()
                .Setup<Task<bool>>("UpdatePaymentStateId", ItExpr.IsAny<string>(), PaymentState.Cancelled)
                .ReturnsAsync(false);

            var result = await businessMock.Object.CancelPayment("PAY-123");

            Assert.False(result);
            businessMock.Protected().Verify("VerifyPaymentAccess", Times.Once(), ItExpr.IsAny<string>());
            businessMock.Protected().Verify("UpdatePaymentStateId", Times.Once(), ItExpr.IsAny<string>(), PaymentState.Cancelled);
        }

        [Fact]
        public async Task CancelPayment_ShouldReturnTrue_WhenUpdatePaymentStateIdSucceeds()
        {
            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                Mock.Of<IPayPalData>(),
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaymentAccess", ItExpr.IsAny<string>())
                .ReturnsAsync(true);

            businessMock.Protected()
                .Setup<Task<bool>>("UpdatePaymentStateId", ItExpr.IsAny<string>(), PaymentState.Cancelled)
                .ReturnsAsync(true);

            var result = await businessMock.Object.CancelPayment("PAY-123");

            Assert.True(result);
            businessMock.Protected().Verify("VerifyPaymentAccess", Times.Once(), ItExpr.IsAny<string>());
            businessMock.Protected().Verify("UpdatePaymentStateId", Times.Once(), ItExpr.IsAny<string>(), PaymentState.Cancelled);
        }

        [Fact]
        public async Task Webhook_ShouldReturnFalse_WhenVerifyPaypalEventFails()
        {
            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                Mock.Of<IPayPalData>(),
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaypalEvent", ItExpr.IsAny<JsonDocument>(), ItExpr.IsAny<IHeaderDictionary>())
                .ReturnsAsync(false);

            var result = await businessMock.Object.Webhook(It.IsAny<JsonDocument>(), It.IsAny<IHeaderDictionary>());

            Assert.False(result);

            businessMock.Protected().Verify("VerifyPaypalEvent", Times.Once(), ItExpr.IsAny<JsonDocument>(), ItExpr.IsAny<IHeaderDictionary>());
            businessMock.Protected().Verify("GetPaymentIdFromBody", Times.Never(), ItExpr.IsAny<JsonDocument>());
            businessMock.Protected().Verify("GetPaymentDetails", Times.Never(), ItExpr.IsAny<string>());
            businessMock.Protected().Verify("UpdatePaymentStateId", Times.Never(), ItExpr.IsAny<string>(), ItExpr.IsAny<PaymentState>());
        }

        [Fact]
        public async Task Webhook_ShouldReturnFalse_WhenGetPaymentIdFromBodyReturnNull()
        {
            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                Mock.Of<IPayPalData>(),
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaypalEvent", ItExpr.IsAny<JsonDocument>(), ItExpr.IsAny<IHeaderDictionary>())
                .ReturnsAsync(true);

            businessMock.Protected()
               .Setup<string>("GetPaymentIdFromBody", ItExpr.IsAny<JsonDocument>())
               .Returns(() => null!);

            var result = await businessMock.Object.Webhook(It.IsAny<JsonDocument>(), It.IsAny<IHeaderDictionary>());

            Assert.False(result);

            businessMock.Protected().Verify("VerifyPaypalEvent", Times.Once(), ItExpr.IsAny<JsonDocument>(), ItExpr.IsAny<IHeaderDictionary>());
            businessMock.Protected().Verify("GetPaymentIdFromBody", Times.Once(), ItExpr.IsAny<JsonDocument>());
            businessMock.Protected().Verify("GetPaymentDetails", Times.Never(), ItExpr.IsAny<string>());
            businessMock.Protected().Verify("UpdatePaymentStateId", Times.Never(), ItExpr.IsAny<string>(), ItExpr.IsAny<PaymentState>());
        }

        [Fact]
        public async Task Webhook_ShouldReturnFalse_WhenGetPaymentDetailsReturnNull()
        {
            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                Mock.Of<IPayPalData>(),
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaypalEvent", ItExpr.IsAny<JsonDocument>(), ItExpr.IsAny<IHeaderDictionary>())
                .ReturnsAsync(true);

            businessMock.Protected()
               .Setup<string>("GetPaymentIdFromBody", ItExpr.IsAny<JsonDocument>())
               .Returns("Fake_PaymentId");

            businessMock.Protected()
               .Setup<Task<PaymentDetails>>("GetPaymentDetails", ItExpr.IsAny<string>())
               .ReturnsAsync(() => null!);

            var result = await businessMock.Object.Webhook(It.IsAny<JsonDocument>(), It.IsAny<IHeaderDictionary>());

            Assert.False(result);

            businessMock.Protected().Verify("VerifyPaypalEvent", Times.Once(), ItExpr.IsAny<JsonDocument>(), ItExpr.IsAny<IHeaderDictionary>());
            businessMock.Protected().Verify("GetPaymentIdFromBody", Times.Once(), ItExpr.IsAny<JsonDocument>());
            businessMock.Protected().Verify("GetPaymentDetails", Times.Once(), ItExpr.IsAny<string>());
            businessMock.Protected().Verify("UpdatePaymentStateId", Times.Never(), ItExpr.IsAny<string>(), ItExpr.IsAny<PaymentState>());
        }

        [Fact]
        public async Task Webhook_ShouldReturnFalse_WhenGetPaymentDetailsOrderIdInvalid()
        {
            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                Mock.Of<IPayPalData>(),
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaypalEvent", ItExpr.IsAny<JsonDocument>(), ItExpr.IsAny<IHeaderDictionary>())
                .ReturnsAsync(true);

            businessMock.Protected()
               .Setup<string>("GetPaymentIdFromBody", ItExpr.IsAny<JsonDocument>())
               .Returns("Fake_PaymentId");

            businessMock.Protected()
               .Setup<Task<PaymentDetails>>("GetPaymentDetails", ItExpr.IsAny<string>())
               .ReturnsAsync(new PaymentDetails { orderId = -10 });


            var result = await businessMock.Object.Webhook(It.IsAny<JsonDocument>(), It.IsAny<IHeaderDictionary>());

            Assert.False(result);

            businessMock.Protected().Verify("VerifyPaypalEvent", Times.Once(), ItExpr.IsAny<JsonDocument>(), ItExpr.IsAny<IHeaderDictionary>());
            businessMock.Protected().Verify("GetPaymentIdFromBody", Times.Once(), ItExpr.IsAny<JsonDocument>());
            businessMock.Protected().Verify("GetPaymentDetails", Times.Once(), ItExpr.IsAny<string>());
            businessMock.Protected().Verify("UpdatePaymentStateId", Times.Never(), ItExpr.IsAny<string>(), ItExpr.IsAny<PaymentState>());
        }

        [Fact]
        public async Task Webhook_ShouldReturnTrue_WhenAllValid()
        {
            var businessMock = new Mock<PayPalBusiness>(
                Mock.Of<PaypalOptions>(),
                Mock.Of<PaypalUrls>(),
                Mock.Of<IPayPalData>(),
                Mock.Of<IOrdersBusiness>(),
                Mock.Of<ILogger<PayPalBusiness>>(),
                Mock.Of<ISalesBusiness>(),
                Mock.Of<ICartItemBusiness>(),
                Mock.Of<IUsersBusiness>()
            )
            { CallBase = true };

            businessMock.Protected()
                .Setup<Task<bool>>("VerifyPaypalEvent", ItExpr.IsAny<JsonDocument>(), ItExpr.IsAny<IHeaderDictionary>())
                .ReturnsAsync(true);

            businessMock.Protected()
               .Setup<string>("GetPaymentIdFromBody", ItExpr.IsAny<JsonDocument>())
               .Returns("Fake_PaymentId");

            businessMock.Protected()
               .Setup<Task<PaymentDetails>>("GetPaymentDetails", ItExpr.IsAny<string>())
               .ReturnsAsync(new PaymentDetails { orderId = 5 });

            businessMock.Protected()
              .Setup<Task<bool>>("UpdatePaymentStateId", ItExpr.IsAny<string>(), ItExpr.IsAny<PaymentState>())
              .ReturnsAsync(true);

            var result = await businessMock.Object.Webhook(It.IsAny<JsonDocument>(), It.IsAny<IHeaderDictionary>());

            Assert.True(result);

            businessMock.Protected().Verify("VerifyPaypalEvent", Times.Once(), ItExpr.IsAny<JsonDocument>(), ItExpr.IsAny<IHeaderDictionary>());
            businessMock.Protected().Verify("GetPaymentIdFromBody", Times.Once(), ItExpr.IsAny<JsonDocument>());
            businessMock.Protected().Verify("GetPaymentDetails", Times.Once(), ItExpr.IsAny<string>());
            businessMock.Protected().Verify("UpdatePaymentStateId", Times.Once(), ItExpr.IsAny<string>(), ItExpr.IsAny<PaymentState>());
        }
    }
}
