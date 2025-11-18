using Models;




namespace Business_Layer.Interfaces;

public interface IPromoCodeBusiness
{
    Task<OperationResult<bool>> AddPromoCode(AddPromocode promoCode);
    Task<OperationResult<bool>> TogglePromocode(int promocodeId);
    Task<OperationResult<List<PromoCode>>> GetPromoCodes();
}
