using Presentation_Layer.Authorization;
using Business_Layer.Interfaces;
using Enums;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation_Layer.Controllers;

[Authorize]
[CheckPermission(Permission.Promocodes_ManagePromocode)]
[ApiController]
[Route("[controller]")]
public class PromocodeController : ControllerBase
{

    public IPromoCodeBusiness PromoCodeBusiness { get; }

    public PromocodeController(IPromoCodeBusiness promoCodeBusiness)
    {
        PromoCodeBusiness = promoCodeBusiness;
    }


    [HttpPost]
    public async Task<IActionResult> AddPromoCode(AddPromocode promoCode)
    {
        var result = await PromoCodeBusiness.AddPromoCode(promoCode);
        return result.Success ?
            Ok() : BadRequest(result.ErrorMessage);
    }



    [HttpGet]
    public async Task<IActionResult> GetMyPromoCodes()
    {
        var result = await PromoCodeBusiness.GetPromoCodes();
        return result == null || result.Count < 1 ?
            BadRequest() : Ok(result);
    }



    [HttpPatch("{id}")]
    public async Task<IActionResult> TogglePromoCode(int id)
    {
        return await PromoCodeBusiness.TogglePromocode(id) ?
            Ok() : BadRequest();
    }

}
