using Models;




namespace Business_Layer.Interfaces;

public interface IPromoCodeBusiness
{
    Task<OperationResult<bool>> AddPromoCode(AddPromocode promoCode);
    Task<bool> TogglePromocode(int promocodeId);
    Task<List<PromoCode>> GetPromoCodes();
}
