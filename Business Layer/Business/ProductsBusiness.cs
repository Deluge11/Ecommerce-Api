

using Business_Layer.Interfaces;
using Data_Layer.Interfaces;
using Business_Layer.Sanitizations;
using Enums;
using Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text;
using Options;
using System.Net;
using System.Net.Http.Headers;

namespace Business_Layer.Business;

public class ProductsBusinees : IProductsBusiness
{
    public IProductsData ProductsData { get; }
    public IImagesBusiness ImagesBusiness { get; }
    public IUsersBusiness UsersBusiness { get; }
    public IInventoryKeyGenerator InventoryKeyGenerator { get; }
    public StoreUrls StoreUrls { get; }
    public HttpClient HttpClient { get; }

    public ProductsBusinees(
        IProductsData productsData,
        IImagesBusiness imagesBusiness,
        IUsersBusiness usersBusiness,
        IInventoryKeyGenerator inventoryKeyGenerator,
        StoreUrls storeUrls,
        HttpClient httpClient
        )
    {
        ProductsData = productsData;
        ImagesBusiness = imagesBusiness;
        UsersBusiness = usersBusiness;
        InventoryKeyGenerator = inventoryKeyGenerator;
        StoreUrls = storeUrls;
        HttpClient = httpClient;
    }


    public async Task<decimal> GetMyProductPriceById(int productId)
    {
        if (productId < 1)
        {
            return -1;
        }

        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return -1;
        }

        return await ProductsData.GetMyProductPriceById(productId, userId);
    }

    public async Task<List<ProductCatalog>> GetMyProducts()
    {
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return [];
        }

        return await ProductsData.GetMyProducts(userId);
    }

    public async Task<ProductDetails> GetProductById(int productId)
    {
        if (productId < 1)
        {
            return null;
        }

        return await ProductsData.GetProductById(productId);
    }

    public async Task<List<ProductCatalog>> GetProductByUserId(int userId)
    {
        if (userId < 1)
        {
            return [];
        }

        return await ProductsData.GetProductByUserId(userId);
    }

    public async Task<List<ProductImage>> GetProductImages(int productId)
    {
        if (productId < 1)
        {
            return [];
        }

        return await ProductsData.GetProductImages(productId);
    }

    public async Task<List<ProductCatalog>> GetProductsCatalog(ProductPageCatalogInfo request)
    {
        if (request.Take < 1 || request.Take > 12)
        {
            return null;
        }
        if (request.CategoryId < 1)
        {
            return null;
        }
        return await ProductsData.GetProductsCatalog(request);
    }

    public async Task<OperationResult<int>> InsertProduct(InsertProductRequest product)
    {
        OperationResult<int> createNewProductOperation = new();
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            createNewProductOperation.ErrorMessage = "invalid user id";
        }
        if (product.price < 1)
        {
            createNewProductOperation.ErrorMessage = "Invalid price value";
        }
        if (product.categoryId < 1)
        {
            createNewProductOperation.ErrorMessage = "Invalid category id value";
        }
        if (product.weight < 0.2m)
        {
            createNewProductOperation.ErrorMessage = "weight value less than 0.2Kg";
        }

        product.name = Sanitization.SanitizeInput(product.name.Trim());
        if (product.name.Length < 1)
        {
            createNewProductOperation.ErrorMessage = "invalid user id";
        }
        if (createNewProductOperation.ErrorMessage != null)
        {
            return createNewProductOperation;
        }

        if (product.description != null)
        {
            product.description = Sanitization.SanitizeInput(product.description.Trim());
        }

        int newProductId = await ProductsData.InsertProduct(product, userId);

        if (newProductId == 0)
        {
            createNewProductOperation.ErrorMessage = "Add products info faild";
            return createNewProductOperation;
        }

        if (!await AddStockIntoStore(product, newProductId, userId))
        {
            createNewProductOperation.ErrorMessage = "Add products info faild";
            return createNewProductOperation;
        }

        createNewProductOperation.Success = true;
        return createNewProductOperation;
    }

    public async Task<bool> AddStockQuantity(AddProductQuantity item)
    {
        if (item.quantity < 1)
            return false;

        if (item.expiryDate < DateTime.UtcNow.AddDays(3))
            return false;

        if (!await ProductsData.IsMyProduct(item.stockId, UsersBusiness.GetUserId()))
            return false;

        return await AddStockQuantityRequest(item);
    }

    public async Task<bool> SetProductMainImage(int productId, int imageId)
    {
        return await ProductsData.SetProductMainImage(productId, UsersBusiness.GetUserId(), imageId);
    }

    public async Task<bool> UpdateProduct(UpdateProductRequest product)
    {
        if (product.price < 1)
            return false;

        if (product.description != null)
        {
            product.description = Sanitization.SanitizeInput(product.description);
        }

        product.name = Sanitization.SanitizeInput(product.name);

        return await ProductsData.UpdateProduct(product, UsersBusiness.GetUserId());
    }

    public async Task<bool> UpdateProductState(int productId, ProductState state)
    {
        bool isValid = Enum.IsDefined(typeof(ProductState), state);

        if (!isValid)
            return false;

        return await ProductsData.UpdateProductState(productId, UsersBusiness.GetUserId(), (int)state);
    }

    public async Task<List<string>> GetProductNames()
    {
        return await ProductsData.GetProductNames();
    }

    public async Task<bool> UploadImage(List<IFormFile> images, int productId)
    {
        int maxImages = 10;

        if (images == null)
            return false;

        if (images.Count == 0)
            return false;

        if (images.Count > maxImages)
            return false;

        if (!await ProductsData.IsMyProduct(productId, UsersBusiness.GetUserId()))
            return false;

        string folderName = Path.Combine("Images/ProductImage", productId.ToString());

        if (!Directory.Exists(folderName))
        {
            Directory.CreateDirectory(folderName);
        }

        int startImageCount = Directory.GetFiles(folderName).Length;


        for (int i = 0; i < images.Count; i++)
        {
            if (i + startImageCount >= maxImages)
            {
                break;
            }

            var file = images[i];

            if (!await ImagesBusiness.IsValidImage(file))
            {
                continue;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var filePath = Path.Combine(folderName, Guid.NewGuid().ToString() + extension);
            int imageId = await ProductsData.SaveImagePath(filePath, productId);

            if (imageId == 0)
            {
                continue;
            }

            await ImagesBusiness.StreamImage(filePath, file);

            if (startImageCount == 0)
            {
                await SetProductMainImage(productId, imageId);
            }
        }

        int folderCountAfterUploading = Directory.GetFiles(folderName).Length;

        return startImageCount != folderCountAfterUploading;
    }

    private async Task<bool> AddStockIntoStore(InsertProductRequest product, int productId, int userId)
    {

        var stock = new Stock
        {
            stockId = productId,
            sellerId = userId,
            weight = product.weight
        };

        try
        {
            string token = InventoryKeyGenerator.GenerateJwt();

            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(stock);
            var body = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync(StoreUrls.AddStock, body);
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> AddStockQuantityRequest(AddProductQuantity request)
    {
        try
        {
            string token = InventoryKeyGenerator.GenerateJwt();

            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(request);
            var body = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync(StoreUrls.AddStockQuantity, body);
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }
}
