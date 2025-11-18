using Enums;
using Microsoft.AspNetCore.Http;
using Models;

namespace Business_Layer.Interfaces;

public interface IProductsBusiness
{
    //Task<List<ProductCatalog>> GetProductsCatalog(ProductPageInfo request);
    Task<List<ProductCatalog>> GetProductsCatalog(ProductPageCatalogInfo request);

    Task<ProductDetails> GetProductById(int productId);
    Task<List<ProductCatalog>> GetProductByUserId(int userId);
    Task<List<ProductCatalog>> GetMyProducts();
    Task<decimal> GetMyProductPriceById(int productId);
    Task<int> InsertProduct(InsertProductRequest product);
    Task<bool> AddStockQuantity(AddProductQuantity item);
    Task<bool> UpdateProduct(UpdateProductRequest product);
    Task<bool> UpdateProductState(int productId, ProductState state);
    Task<bool> UploadImage(List<IFormFile> images, int productId);
    Task<List<ProductImage>> GetProductImages(int productId);
    Task<bool> SetProductMainImage(int productId, int imageId);
    Task<List<string>> GetProductNames();
}
