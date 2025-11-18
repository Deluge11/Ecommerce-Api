using Enums;
using Data_Layer.Interfaces;
using Business_Layer.Interfaces;
using Business_Layer.Sanitizations;
using Models;

namespace Business_Layer.Business;

public class PromoCodeBusiness : IPromoCodeBusiness
{
    public IProductsBusiness ProductsBusiness { get; }
    public IPromoCodeData PromoCodeData { get; }
    public IUsersBusiness UsersBusiness { get; }

    public PromoCodeBusiness(
        IProductsBusiness productsBusiness, 
        IPromoCodeData promoCodeData,
        IUsersBusiness usersBusiness
        )
    {
        ProductsBusiness = productsBusiness;
        PromoCodeData = promoCodeData;
        UsersBusiness = usersBusiness;
    }


    public async Task<OperationResult<bool>> AddPromoCode(AddPromocode promoCode)
    {
        var result = new OperationResult<bool>();

        string verifyResult = await VerifyPromoCode(promoCode);

        if (!string.IsNullOrEmpty(verifyResult))
        {
            result.ErrorMessage = verifyResult;
            result.ErrorType = ErrorType.BadRequest;
            result.IsSuccess = false;
            return result;
        }

        promoCode.code = Sanitization.SanitizeInput(promoCode.code);

        return await PromoCodeData.AddPromoCode(promoCode, UsersBusiness.GetUserId());
    }

    public async Task<OperationResult<List<PromoCode>>> GetPromoCodes()
    {
        return await PromoCodeData.GetPromoCodes(UsersBusiness.GetUserId());
    }

    public async Task<OperationResult<bool>> TogglePromocode(int promocodeId)
    {
        return await PromoCodeData.TogglePromocode(promocodeId, UsersBusiness.GetUserId());
    }

    private async Task<string> VerifyPromoCode(AddPromocode promoCode)
    {
        decimal productPrice = await ProductsBusiness.GetMyProductPriceById(promoCode.productId);

        if (productPrice == -1)
        {
            return "Product Not Found!";
        }
        if (promoCode.expiryDate < DateTime.UtcNow.AddHours(1))
        {
            return "Expiry date Must be 1 hours older than the current time";
        }
        if (promoCode.discount < 0.5m)
        {
            return "Discount must be greater than 0.5";
        }
        if (!Enum.IsDefined(typeof(DiscountType), promoCode.discountType))
        {
            return "Invalid Discount Type";
        }
        if (promoCode.discountType == DiscountType.Fixed && productPrice - promoCode.discount < 1)
        {
            return "Price after discount must be 1 Dollar at least";
        }
        if (promoCode.discountType == DiscountType.Percent && productPrice - promoCode.discount / 100 < 1)
        {
            return "Price after discount must be 1 Dollar at least";
        }
        if (promoCode.count < 1 || promoCode.count > 1000)
        {
            return "count must be between 1 - 1000";
        }
        if (promoCode.code.Length < 5 || promoCode.code.Length > 12)
        {
            return "Code length must be between 5 - 12 letters";
        }
        if (promoCode.code.Contains(" "))
        {
            return "Code text should not contains Spaces";
        }

        return string.Empty;
    }
}
