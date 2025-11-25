
using Business_Layer.Interfaces;
using Data_Layer.Interfaces;
using Models;
namespace Business_Layer.Business;

public class CartsBusiness : ICartsBusiness
{
    public ICartsData CartsData { get; }
    public IUsersBusiness UsersBusiness { get; }

    public CartsBusiness(ICartsData cartsData, IUsersBusiness usersBusiness)
    {
        CartsData = cartsData;
        UsersBusiness = usersBusiness;
    }


    public async Task<List<CartItemCatalog>> GetCartItems()
    {
        int userId = UsersBusiness.GetUserId();

        if(userId == 0)
        {
            return [];
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
