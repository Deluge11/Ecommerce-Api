using Enums;
using Presentation_Layer.Authorization;
using Business_Layer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Presentation_Layer.Controllers;


[ApiController]
[Route("[controller]")]
[Authorize]
[CheckPermission(Permission.Carts_ManageCart)]
public class CartsController : ControllerBase
{
    public ICartsBusiness CartsBusiness { get; }

    public CartsController(ICartsBusiness cartsBusiness)
    {
        CartsBusiness = cartsBusiness;
    }



    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCartItemsCount()
    {
        var count = await CartsBusiness.GetCartItemsCount();
        return count != -1 ?
            Ok(count) : StatusCode(500);
    }



    [HttpGet("items")]
    public async Task<ActionResult<List<CartItemCatalog>>> GetCartItems()
    {
        var items = await CartsBusiness.GetCartItems();
        return items != null ?
            Ok(items) : StatusCode(500);
    }



    [HttpGet]
    public async Task<ActionResult<decimal>> GetTotalPrice()
    {
        var totalPrice = await CartsBusiness.GetTotalPrice();
        return totalPrice != -1 ?
            Ok(totalPrice) : StatusCode(500);
    }



}
