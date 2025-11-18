
using System.Data;
using Models;

namespace Data_Layer.Interfaces;

public interface ICartItemData
{
    Task<bool> InsertCartItem(int productId, int userId);
    Task<bool> UpdateCartItem(int cartItemId, int count, int userId);
    Task<bool> DeleteCartItem(int itemId, int userId);
    Task<bool> SyncCartItemsPromocode(int userId);
    Task<bool> UsePromocode(int productId, string promocode, int userId);
    Task<List<NewOrderRequest>> GetCartItemQuantities(int userId);
    Task<bool> SyncCartItemsCount(DataTable items, int userId);
}
