using Business_Layer.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation_Layer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SellersController : ControllerBase
    {
        public SellerBusiness SellerBusiness { get; }
        public SellersController(SellerBusiness sellerBusiness)
        {
            SellerBusiness = sellerBusiness;
        }


        [HttpPost]
        public async Task<IActionResult> ApplyForSeller()
        {
            return await SellerBusiness.ApplyForSeller() ?
                Ok() : BadRequest();
        }

        //Admin
        [HttpPatch("confirm/{requestId}")]
        public async Task<IActionResult> ConfirmSeller(int requestId)
        {
            return await SellerBusiness.ConfirmSeller(requestId) ?
                Ok() : BadRequest();
        }

        //Admin
        [HttpPatch("refuse/{requestId}")]
        public async Task<IActionResult> RefuseSeller(int requestId)
        {
            return await SellerBusiness.RefuseSeller(requestId) ?
                Ok() : BadRequest();
        }
    }
}
