
using Models;
namespace Business_Layer.Interfaces;

public interface ICartItemBusiness
{
    Task<bool> InsertCartItem(int productId);
    Task<bool> PlusOneCartItem(int cartItemId);
    Task<bool> MinusOneCartItem(int cartItemId);
    Task<bool> DeleteCartItem(int itemId);
    Task<bool> SyncCartItemsPromocode();
    Task<bool> UsePromocode(int productId, string promocode);
    Task<List<NewOrderRequest>> GetCartItemQuantities();
    Task<bool> SyncCartItemsCount(List<NewOrderRequest> items);
    Task<bool> SyncCartItemsWithStocks();
}
