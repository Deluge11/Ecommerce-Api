
using Microsoft.Data.SqlClient;
using System.Data;
using Models;

namespace Data_Layer.Interfaces;

public interface ICartsData
{
    Task<int> GetCartItemsCount(int userId);
    Task<List<CartItemCatalog>> GetCartItems(int userId);
    Task<decimal> GetTotalPrice(int userId);
}
