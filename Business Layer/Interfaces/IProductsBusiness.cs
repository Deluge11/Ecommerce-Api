using Enums;
using Microsoft.AspNetCore.Http;
using Models;

namespace Business_Layer.Interfaces;

public interface IProductsBusiness
{
    Task<List<ProductCatalog>> GetProductsCatalog(int categoryId, int take, int lastSeenId);
    Task<ProductDetails> GetProductById(int productId);
    Task<List<ProductCatalog>> GetProductByUserId(int userId);
    Task<List<ProductCatalog>> GetMyProducts();
    Task<decimal> GetMyProductPriceById(int productId);
    Task<OperationResult<int>> InsertProduct(InsertProductRequest product);
    Task<bool> AddStockQuantity(AddProductQuantity item);
    Task<bool> UpdateProduct(UpdateProductRequest product);
    Task<bool> UpdateProductState(int productId, ProductState state);
    Task<OperationResult<string>> UploadImage(List<IFormFile> images, int productId);
    Task<List<ProductImage>> GetProductImages(int productId);
    Task<bool> SetProductMainImage(int productId, int imageId);
    Task<List<string>> GetProductNames();
}
