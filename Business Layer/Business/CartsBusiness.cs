

using Data_Layer.Data;
using Models;
namespace Business_Layer.Business;

public class CartsBusiness
{
    public CartsData CartsData { get; }
    public UsersBusiness UsersBusiness { get; }

    public CartsBusiness(CartsData cartsData, UsersBusiness usersBusiness)
    {
        CartsData = cartsData;
        UsersBusiness = usersBusiness;
    }


    public async Task<List<CartItemCatalog>> GetCartItems()
    {
        int userId = UsersBusiness.GetUserId();

        if(userId == 0)
        {
            return null;
        }

        return await CartsData.GetCartItems(userId);
    }

    public async Task<int> GetCartItemsCount()
    {
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return 0;
        }

        return await CartsData.GetCartItemsCount(userId);
    }

    public async Task<decimal> GetTotalPrice()
    {
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return 0;
        }

        return await CartsData.GetTotalPrice(userId);
    }

}
