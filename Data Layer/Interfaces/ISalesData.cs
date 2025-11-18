using Models;



namespace Data_Layer.Interfaces;

public interface ISalesData
{
    Task<List<SalesCatalog>> GetMySales(int stateId, int userId, int lastSeenId);
    Task<List<SalesDetails>> GetAllSales(int stateId, int lastSeenId);
    Task<decimal?> GetSalesProfits(int sellerId);
    Task<List<MerchantAccounting>> GetMerchantAccounting(int stateId, int lastIdSeen);
    Task<List<MerchantAccountingDetails>> GetNewMerchantAccountingDetails();
    Task UpdateMerchantAccountState(int id, int stateId);
}
