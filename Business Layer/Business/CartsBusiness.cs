
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
        return await CartsData.GetCartItems(UsersBusiness.GetUserId());
    }

    public async Task<int> GetCartItemsCount()
    {
        return await CartsData.GetCartItemsCount(UsersBusiness.GetUserId());
    }

    public async Task<decimal> GetTotalPrice()
    {
        return await CartsData.GetTotalPrice(UsersBusiness.GetUserId());
    }

}
