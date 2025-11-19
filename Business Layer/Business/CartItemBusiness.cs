using Data_Layer.Interfaces;
using Business_Layer.Interfaces;
using System.Data;
using System.Text.Json;
using System.Text;
using Models;
using Options;

namespace Business_Layer.Business;

public class CartItemBusiness : ICartItemBusiness
{
    public ICartItemData CartItemData { get; }
    public IUsersBusiness UsersBusiness { get; }
    public StoreUrls StoreUrls { get; }
    public HttpClient HttpClient { get; }

    public CartItemBusiness(
        ICartItemData cartItemData,
        IUsersBusiness usersBusiness,
        StoreUrls storeUrls,
        HttpClient httpClient
        )
    {
        CartItemData = cartItemData;
        UsersBusiness = usersBusiness;
        StoreUrls = storeUrls;
        HttpClient = httpClient;
    }


    public async Task<bool> SyncCartItemsPromocode()
    {
       return await CartItemData.SyncCartItemsPromocode(UsersBusiness.GetUserId());
    }

    public async Task<bool> DeleteCartItem(int itemId)
    {
        return await CartItemData.DeleteCartItem(itemId, UsersBusiness.GetUserId());
    }

    public async Task<bool> InsertCartItem(int productId)
    {
        return await CartItemData.InsertCartItem(productId, UsersBusiness.GetUserId());
    }

    public async Task<bool> PlusOneCartItem(int cartItemId)
    {
        return await CartItemData.UpdateCartItem(cartItemId, 1, UsersBusiness.GetUserId());
    }
    public async Task<bool> MinusOneCartItem(int cartItemId)
    {
        return await CartItemData.UpdateCartItem(cartItemId, -1, UsersBusiness.GetUserId());
    }

    public async Task<bool> UsePromocode(int productId, string promocode)
    {
        return await CartItemData.UsePromocode(productId, promocode, UsersBusiness.GetUserId());
    }

    public async Task<List<NewOrderRequest>> GetCartItemQuantities()
    {
        return await CartItemData.GetCartItemQuantities(UsersBusiness.GetUserId());
    }

    public async Task<bool> SyncCartItemsCount(List<NewOrderRequest> items)
    {
        return await CartItemData.SyncCartItemsCount(ToDataTable(items), UsersBusiness.GetUserId());
    }

    public async Task<bool> SyncCartItemsWithStocks()
    {
        int userId = UsersBusiness.GetUserId();

        var cartItemsQuantities = await GetCartItemQuantities();

        try
        {
            var json = JsonSerializer.Serialize(cartItemsQuantities);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await HttpClient.PatchAsync(StoreUrls.SyncOrderRequest, content);

            if (!response.IsSuccessStatusCode)
                return false;

            var responseJson = await response.Content.ReadAsStringAsync();
            var modifiedItems = JsonSerializer.Deserialize<List<NewOrderRequest>>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if(modifiedItems == null)
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

    private DataTable ToDataTable(List<NewOrderRequest> items)
    {
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
