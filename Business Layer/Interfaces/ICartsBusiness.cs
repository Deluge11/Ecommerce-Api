
using Microsoft.Data.SqlClient;
using System.Data;
using Models;

namespace Business_Layer.Interfaces;

public interface ICartsBusiness
{
    Task<int> GetCartItemsCount();
    Task<List<CartItemCatalog>> GetCartItems();
    Task<decimal> GetTotalPrice();
}
