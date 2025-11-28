using Models;


namespace Data_Layer.Interfaces;

public interface IPromoCodeData
{
    Task<OperationResult<bool>> AddPromoCode(AddPromocode promoCode,int userId);
    Task<bool> TogglePromocode(int promocodeId, int userId);
    Task<List<PromoCode>> GetPromoCodes(int userId);
}
