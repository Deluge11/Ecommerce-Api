using Models;



namespace Data_Layer.Interfaces;

public interface IProductsData
{
    //Task<List<ProductCatalog>> GetProductsCatalog(ProductPageInfo request);
    Task<List<ProductCatalog>> GetProductsCatalog(int categoryId, int take, int lastSeenId);
    Task<List<ProductCatalog>> GetProductsCatalogForAllCategories(int take, int lastSeenId);
    Task<ProductDetails> GetProductById(int productId);
    Task<List<ProductCatalog>> GetProductByUserId(int userId);
    Task<List<ProductCatalog>> GetMyProducts(int userId);
    Task<decimal> GetMyProductPriceById(int productId, int userId);
    Task<int> InsertProduct(InsertProductRequest product, int userId);
    Task<bool> UpdateProduct(UpdateProductRequest product, int userId);
    Task<bool> UpdateProductState(int productId, int userId, int state);
    Task<List<ProductImage>> GetProductImages(int productId);
    Task<bool> SetProductMainImage(int productId, int userId, int imageId);
    Task<bool> IsMyProduct(int productId, int userId);
    Task<int> SaveImagePath(string filePath, int productId);
    Task<List<string>> GetProductNames();
}
