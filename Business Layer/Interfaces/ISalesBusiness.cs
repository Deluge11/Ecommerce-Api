using Enums;
using Models;



namespace Business_Layer.Interfaces;

public interface ISalesBusiness
{
    Task<List<SalesCatalog>> GetMySales(SalesState state, int lastSeenId);
    Task<List<SalesDetails>> GetAllSales(SalesState state, int lastSeenId);
    Task<decimal?> GetSalesProfits(int sellerId);
    Task<List<MerchantAccounting>> GetMerchantAccounting(MerchantAccountingState state, int lastIdSeen);
    Task<List<MerchantAccountingDetails>> GetNewMerchantAccountingDetails();
    Task UpdateMerchantAccountState(int id, MerchantAccountingState state);
}
