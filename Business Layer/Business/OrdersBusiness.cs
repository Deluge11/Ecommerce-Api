
using System.Text;
using System.Text.Json;
using Business_Layer.Interfaces;
using Data_Layer.Interfaces;
using Options;
using Models;
using Data_Layer.Data;
using System.Net.Http.Headers;
using System.ClientModel.Primitives;

namespace Business_Layer.Business;

public class OrdersBusiness : IOrdersBusiness
{
    public OrdersBusiness(
        IOrdersData ordersData,
        IUsersBusiness usersBusiness,
        ICartItemBusiness cartItemBusiness,
        IInventoryKeyGenerator inventoryKeyGenerator,
        StoreUrls storeUrls,
        HttpClient httpClient
        )
    {
        OrdersData = ordersData;
        StoreUrls = storeUrls;
        UsersBusiness = usersBusiness;
        CartItemBusiness = cartItemBusiness;
        InventoryKeyGenerator = inventoryKeyGenerator;
        HttpClient = httpClient;
    }

    public IOrdersData OrdersData { get; }
    public StoreUrls StoreUrls { get; }
    public IUsersBusiness UsersBusiness { get; }
    public ICartItemBusiness CartItemBusiness { get; }
    public IInventoryKeyGenerator InventoryKeyGenerator { get; }
    public HttpClient HttpClient { get; }
    public InventoryOptions InventoryOptions { get; }



    public async Task<OperationResult<Order>> CreateOrder()
    {
        OperationResult<Order> operationResult = new();
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            operationResult.ErrorMessage = "Invalid user id";
            return operationResult;
        }


        Order order = await OrdersData.CreateOrder(userId);

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

        bool BookedOrderSuccess = await CreateStoreOrder(order.Id);

        if (!BookedOrderSuccess)
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

        operationResult.Success = true;
        operationResult.Data = order;
        return operationResult;
    }


    public async Task<Order> GetOrderById(int orderId)
    {
        if (orderId < 1)
        {
            return null;
        }

        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return null;
        }

        return await OrdersData.GetOrderById(orderId, userId);
    }

    public async Task<List<OrderDetails>> GetOrderDetails(int orderId)
    {
        if (orderId < 1)
        {
            return null;
        }

        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return null;
        }

        return await OrdersData.GetOrderDetails(orderId, userId);
    }

    public async Task<List<Order>> GetMyOrders()
    {
        int userId = UsersBusiness.GetUserId();

        if (userId <= 0)
        {
            return [];
        }

        return await OrdersData.GetOrdersByUserId(userId);
    }

    public async Task<List<Order>> GetOrdersByUserId(int userId)
    {
        if (userId <= 0)
        {
            return [];
        }

        return await OrdersData.GetOrdersByUserId(userId);
    }
    public async Task<bool> CreateStoreOrder(int orderId)
    {
        if(orderId <= 0)
        {
            return false;
        }

        var items = await GetOrderItemQuantities(orderId);

        try
        {
            string token = InventoryKeyGenerator.GenerateJwt();

            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(items);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync(StoreUrls.BookingStocksRequest + $"{orderId}", content);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ConfrimOrderInStore(int orderId)
    {
        if (orderId <= 0)
        {
            return false;
        }

        try
        {
            string token = InventoryKeyGenerator.GenerateJwt();

            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await HttpClient.PatchAsync(StoreUrls.ConfrimOrder + $"{orderId}", null);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }

    private async Task<List<NewOrderRequest>> GetOrderItemQuantities(int orderId)
    {
        if (orderId <= 0)
        {
            return [];
        }

        return await OrdersData.GetOrderItemQuantities(orderId);
    }

   
}