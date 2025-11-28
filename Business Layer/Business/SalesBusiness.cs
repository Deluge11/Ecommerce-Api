using Business_Layer.Interfaces;
using Data_Layer.Interfaces;
using Enums;
using Models;


namespace Business_Layer.Business;

public class SalesBusiness : ISalesBusiness
{
    private ISalesData SalesData { get; }
    public IUsersBusiness UsersBusiness { get; }

    public SalesBusiness(ISalesData salesData, IUsersBusiness usersBusiness)
    {
        SalesData = salesData;
        UsersBusiness = usersBusiness;
    }


    public async Task<List<SalesDetails>> GetAllSales(SalesState state, int lastSeenId)
    {
        if (!Enum.IsDefined(typeof(SalesState), state))
        {
            return null;
        }

        return await SalesData.GetAllSales((int)state, lastSeenId);
    }

    public async Task<List<MerchantAccounting>> GetMerchantAccounting(MerchantAccountingState state, int lastIdSeen)
    {
        if (!Enum.IsDefined(typeof(MerchantAccountingState), state))
        {
            return null;
        }

        return await SalesData.GetMerchantAccounting((int)state, lastIdSeen);
    }

    public async Task<List<SalesCatalog>> GetMySales(SalesState state, int lastSeenId)
    {
        int userId = UsersBusiness.GetUserId();
        if (userId == 0)
        {
            return null;
        }

        if (!Enum.IsDefined(typeof(SalesState), state))
        {
            return null;
        }

        return await SalesData.GetMySales((int)state, userId, lastSeenId);
    }

    public async Task<List<MerchantAccountingDetails>> GetNewMerchantAccountingDetails()
    {
        return await SalesData.GetNewMerchantAccountingDetails();
    }

    public async Task<decimal?> GetSalesProfits(int sellerId)
    {
        if (sellerId < 1)
        {
            return 0;
        }

        return await SalesData.GetSalesProfits(sellerId);
    }

    public async Task UpdateMerchantAccountState(int id, MerchantAccountingState state)
    {
        if (!Enum.IsDefined(typeof(MerchantAccountingState), state))
        {
            return;
        }

        if(id < 1)
        {
            return;
        }

        await SalesData.UpdateMerchantAccountState(id, (int)state);
    }
}
