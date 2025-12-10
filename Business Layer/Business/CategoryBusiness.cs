using Business_Layer.Interfaces;
using Data_Layer.Interfaces;
using Business_Layer.Sanitizations;
using Microsoft.Extensions.Caching.Memory;
using Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Options;

namespace Business_Layer.Business;

public class CategoryBusiness : ICategoryBusiness
{
    public ICategoryData CategoryData { get; }
    public IImagesBusiness ImagesBusiness { get; }
    public IMemoryCache Cache { get; }
    public CacheKeys CacheKeys { get; }

    public CategoryBusiness(
        ICategoryData categoryData,
        IImagesBusiness imagesBusiness,
        IMemoryCache cache,
        CacheKeys cacheKeys
        )
    {
        CategoryData = categoryData;
        ImagesBusiness = imagesBusiness;
        Cache = cache;
        CacheKeys = cacheKeys;
    }

    public async Task<List<Category>> GetAll()
    {
        return await GetCategoriesList();
    }

    public async Task<bool> Add(string name)
    {
        name = Sanitization.SanitizeInput(name.Trim());

        if (name.Length < 1)
        {
            return false;
        }

        Cache.Remove(CacheKeys.CategoryKey);
        return await CategoryData.Add(name);
    }

    public async Task<bool> AddImage(int categoryId, IFormFile image)
    {
        if (categoryId < 1)
        {
            return false;
        }

        var category = await GetById(categoryId);

        if (image == null || category.id == 0)
        {
            return false;
        }

        if (!await ImagesBusiness.IsValidImage(image))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(category.image) && File.Exists(category.image))
        {
            File.Delete(category.image);
        }

        string folderName = "Images/CategoryImage";
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        var filePath = Path.Combine(folderName, Guid.NewGuid().ToString() + extension);

        Cache.Remove(CacheKeys.CategoryKey);
        await ImagesBusiness.StreamImage(filePath, image);
        await CategoryData.SetCategoryImage(filePath, categoryId);
        return true;
    }

    public async Task<Category> GetById(int id)
    {
        if (id < 1)
        {
            return null;
        }

        var categories = await GetCategoriesList();

        if (id >= categories.Count)
        {
            return null;
        }

        return categories[id];
    }

    public async Task<bool> Update(int categoyId, string categoryName)
    {
        categoryName = Sanitization.SanitizeInput(categoryName.Trim());

        if (categoryName.Length < 1)
        {
            return false;
        }

        Cache.Remove(CacheKeys.CategoryKey);
        return await CategoryData.Update(categoyId, categoryName);
    }

    private async Task<List<Category>> GetCategoriesList()
    {
        var categories = Cache.Get<List<Category>>(CacheKeys.CategoryKey);

        if (categories == null)
        {
            categories = await CategoryData.GetAll() ?? new List<Category>();

            var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1));

            Cache.Set(CacheKeys.CategoryKey, categories, cacheOptions);
        }

        return categories;
    }
}
