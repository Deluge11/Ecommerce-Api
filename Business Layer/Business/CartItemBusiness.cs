using Data_Layer.Interfaces;
using Business_Layer.Interfaces;
using System.Data;
using System.Text.Json;
using System.Text;
using Models;
using Options;
using System.Net.Http.Headers;

namespace Business_Layer.Business;

public class CartItemBusiness : ICartItemBusiness
{
    public ICartItemData CartItemData { get; }
    public IUsersBusiness UsersBusiness { get; }
    public IInventoryKeyGenerator InventoryKeyGenerator { get; }
    public StoreUrls StoreUrls { get; }
    public HttpClient HttpClient { get; }

    public CartItemBusiness(
        ICartItemData cartItemData,
        IUsersBusiness usersBusiness,
        IInventoryKeyGenerator inventoryKeyGenerator,
        StoreUrls storeUrls,
        HttpClient httpClient
        )
    {
        CartItemData = cartItemData;
        UsersBusiness = usersBusiness;
        InventoryKeyGenerator = inventoryKeyGenerator;
        StoreUrls = storeUrls;
        HttpClient = httpClient;
    }


    public async Task<bool> SyncCartItemsPromocode()
    {
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return false;
        }

        return await CartItemData.SyncCartItemsPromocode(userId);
    }

    public async Task<bool> DeleteCartItem(int itemId)
    {
        if (itemId < 1)
        {
            return false;
        }

        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return false;
        }

        return await CartItemData.DeleteCartItem(itemId, userId);
    }

    public async Task<bool> InsertCartItem(int productId)
    {
        if (productId < 1)
        {
            return false;
        }

        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return false;
        }
        return await CartItemData.InsertCartItem(productId, userId);
    }

    public async Task<bool> PlusOneCartItem(int cartItemId)
    {
        if (cartItemId < 1)
        {
            return false;
        }

        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return false;
        }

        return await CartItemData.UpdateCartItem(cartItemId, 1, userId);
    }
    public async Task<bool> MinusOneCartItem(int cartItemId)
    {
        if (cartItemId < 1)
        {
            return false;
        }

        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return false;
        }

        return await CartItemData.UpdateCartItem(cartItemId, -1, userId);
    }

    public async Task<bool> UsePromocode(int productId, string promocode)
    {
        if (promocode == null)
        {
            return false;
        }

        promocode = promocode.Trim();

        if (promocode.Length < 1 || productId < 1)
        {
            return false;
        }

        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return false;
        }

        return await CartItemData.UsePromocode(productId, promocode, userId);
    }

    public async Task<List<NewOrderRequest>> GetCartItemQuantities()
    {
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return null;
        }

        return await CartItemData.GetCartItemQuantities(userId);
    }

    public async Task<bool> SyncCartItemsCount(List<NewOrderRequest> items)
    {
        if (items == null || items.Count < 1)
        {
            return false;
        }

        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return false;
        }

        DataTable cartItemsTable = ToDataTable(items);

        if (cartItemsTable == null)
        {
            return false;
        }

        return await CartItemData.SyncCartItemsCount(cartItemsTable, userId);
    }

    public async Task<bool> SyncCartItemsWithStocks()
    {
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return false;
        }

        var cartItemsQuantities = await GetCartItemQuantities();

        if (cartItemsQuantities == null || cartItemsQuantities.Count < 1)
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
            var json = JsonSerializer.Serialize(cartItemsQuantities);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await HttpClient.PatchAsync(StoreUrls.SyncOrderRequest, content);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var responseJson = await response.Content.ReadAsStringAsync();
            var modifiedItems = JsonSerializer.Deserialize<List<NewOrderRequest>>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (modifiedItems == null)
            {
                return false;
            }

            return await SyncCartItemsCount(modifiedItems);
        }
        catch
        {
            return false;
        }
    }

    protected DataTable ToDataTable(List<NewOrderRequest> items)
    {
        if (items == null || items.Count < 1)
        {
            return null;
        }

        var table = new DataTable();
        table.Columns.Add("mappingProductId", typeof(int));
        table.Columns.Add("quantity", typeof(int));

        foreach (var item in items)
        {
            table.Rows.Add(item.StockId, item.Quantity);
        }

        return table;
    }
}
