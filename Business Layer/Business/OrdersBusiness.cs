
using System.Text;
using System.Text.Json;
using Business_Layer.Interfaces;
using Data_Layer.Interfaces;
using Options;
using Models;
using Data_Layer.Data;
using Business_Layer.Key_Generator_Service;
using System.Net.Http.Headers;

namespace Business_Layer.Business;

public class OrdersBusiness : IOrdersBusiness
{
    public OrdersBusiness(
        IOrdersData ordersData,
        IUsersBusiness usersBusiness,
        StoreUrls storeUrls,
        HttpClient httpClient,
        InventoryOptions inventoryOptions
        )
    {
        OrdersData = ordersData;
        StoreUrls = storeUrls;
        UsersBusiness = usersBusiness;
        HttpClient = httpClient;
        InventoryOptions = inventoryOptions;
    }

    public IOrdersData OrdersData { get; }
    public StoreUrls StoreUrls { get; }
    public IUsersBusiness UsersBusiness { get; }
    public HttpClient HttpClient { get; }
    public InventoryOptions InventoryOptions { get; }

    public async Task<Order> CreateOrder()
    {
        return await OrdersData.CreateOrder(UsersBusiness.GetUserId());
    }

    public async Task<Order> GetOrderById(int orderId)
    {
        return await OrdersData.GetOrderById(orderId, UsersBusiness.GetUserId());
    }

    public async Task<List<OrderDetails>> GetOrderDetails(int orderId)
    {
        return await OrdersData.GetOrderDetails(orderId, UsersBusiness.GetUserId());
    }

    public async Task<List<Order>> GetOrdersByUserId(int userId)
    {
        return await OrdersData.GetOrdersByUserId(userId);
    }
    public async Task<bool> CreateStoreOrder(int orderId)
    {
        var items = await GetOrderItemQuantities(orderId);

        try
        {
            string token = InventoryKeyGenerator.GenerateJwt(
                InventoryOptions.Key, InventoryOptions.Issuer, InventoryOptions.Audience);

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
        try
        {
            string token = InventoryKeyGenerator.GenerateJwt(
             InventoryOptions.Key, InventoryOptions.Issuer, InventoryOptions.Audience);

            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

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
        return await OrdersData.GetOrderItemQuantities(orderId);
    }
}