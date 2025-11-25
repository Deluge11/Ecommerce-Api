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

        if (result.Success)
        {
            return Created();
        }

        if (result.ErrorType == ErrorType.BadRequest)
        {
            return BadRequest(result.ErrorMessage);
        }

        return NotFound("Something went wrong");
    }



    [HttpGet]
    public async Task<IActionResult> GetMyPromoCodes()
    {
        var result = await PromoCodeBusiness.GetPromoCodes();

        if (result.Success)
        {
            return Ok(result.Data);
        }

        return NotFound("Something went wrong");
    }



    [HttpPatch("{id}")]
    public async Task<IActionResult> TogglePromoCode(int id)
    {
        var result = await PromoCodeBusiness.TogglePromocode(id);

        if (result.Success)
        {
            return Ok();
        }

        return NotFound("Something went wrong");
    }

}
