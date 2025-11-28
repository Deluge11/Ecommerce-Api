using Presentation_Layer.Authorization;
using Enums;
using Business_Layer.Interfaces;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Presentation_Layer.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    public IProductsBusiness ProductsBusiness { get; }

    public ProductsController(IProductsBusiness productsBusiness)
    {
        ProductsBusiness = productsBusiness;
    }



    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<ProductCatalog>>> GetProductsPage(ProductPageCatalogInfo request)
    {
        var products = await ProductsBusiness.GetProductsCatalog(request);
        return products != null ?
            Ok(products) : BadRequest();
    }



    [AllowAnonymous]
    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductDetails>> GetProductById(int productId)
    {
        var product = await ProductsBusiness.GetProductById(productId);

        return product != null ?
            Ok(product) : NotFound();
    }



    [AllowAnonymous]
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<ProductCatalog>>> GetProductsByUserId(int userId)
    {
        var products = await ProductsBusiness.GetProductByUserId(userId);
        return products != null ?
            Ok(products) : NotFound();
    }



    [AllowAnonymous]
    [HttpGet("images/{productId}")]
    public async Task<ActionResult<List<ProductImage>>> GetImages(int productId)
    {
        var images = await ProductsBusiness.GetProductImages(productId);
        return images != null ?
            Ok(images) : NotFound();
    }



    [Authorize]
    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpGet("my-products")]
    public async Task<ActionResult<List<ProductCatalog>>> GetMyProducts()
    {
        var products = await ProductsBusiness.GetMyProducts();
        return products != null ?
            Ok(products) : NotFound();
    }



    [Authorize]
    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPost]
    public async Task<ActionResult<int>> InsertProduct(InsertProductRequest product)
    {

        var result = await ProductsBusiness.InsertProduct(product);
        return result.Success ?
            Ok(result) : BadRequest(result.ErrorMessage);
    }



    [Authorize]
    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPost("add-quantity")]
    public async Task<IActionResult> AddStockQuantity(AddProductQuantity request)
    {
        return await ProductsBusiness.AddStockQuantity(request) ?
            Ok() : BadRequest();
    }



    [Authorize]
    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPost("{productId}")]
    public async Task<ActionResult> UploadImage(List<IFormFile> images, int productId)
    {
        var result = await ProductsBusiness.UploadImage(images, productId);
        return result.Success ?
            Ok(result.Data) : BadRequest(result.ErrorMessage);
    }



    [Authorize]
    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPut]
    public async Task<ActionResult> UpdateProduct(UpdateProductRequest product)
    {
        return await ProductsBusiness.UpdateProduct(product) ?
            Ok() : BadRequest("Cant access this product");

    }



    [Authorize]
    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPatch("image/{productId}")]
    public async Task<ActionResult> SetMainImage(int productId, [FromQuery] int imageId)
    {
        return await ProductsBusiness.SetProductMainImage(productId, imageId) ?
            Ok() : BadRequest();
    }



    [Authorize]
    [CheckPermission(Permission.Products_ChangeProductState)]
    [HttpPatch("{productId}")]
    public async Task<ActionResult> UpdateProductState(int productId, ProductState state)
    {
        return await ProductsBusiness.UpdateProductState(productId, state) ?
            Ok() : BadRequest();
    }


}
