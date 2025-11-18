using Business_Layer.Interfaces;
using Data_Layer.Interfaces;
using Business_Layer.Sanitizations;

using Models;
using Microsoft.AspNetCore.Http;

namespace Business_Layer.Business;

public class CategoryBusiness : ICategoryBusiness
{
    public ICategoryData CategoryData { get; }
    public IImagesBusiness ImagesBusiness { get; }

    public CategoryBusiness(ICategoryData categoryData, IImagesBusiness imagesBusiness)
    {
        CategoryData = categoryData;
        ImagesBusiness = imagesBusiness;
    }


    public async Task<bool> Add(string name)
    {
        name = Sanitization.SanitizeInput(name);
        return await CategoryData.Add(name);
    }

    public async Task<bool> AddImage(int categoryId, IFormFile image)
    {
        var category = await GetById(categoryId);

        if (image == null || category.id == 0)
            return false;

        if (!await ImagesBusiness.IsValidImage(image))
            return false;

        if (category.image != null)
            File.Delete(category.image);


        string folderName = "Images/CategoryImage";
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        var filePath = Path.Combine(folderName, Guid.NewGuid().ToString() + extension);

        await ImagesBusiness.StreamImage(filePath, image);
        await CategoryData.SetCategoryImage(filePath, categoryId);
        return true;
    }

    public async Task<List<Category>> GetAll()
    {
        return await CategoryData.GetAll();
    }

    public async Task<Category> GetById(int id)
    {
        return await CategoryData.GetById(id);
    }

    public async Task<bool> Update(int categoyId, string categoryName)
    {
        categoryName = Sanitization.SanitizeInput(categoryName);
        return await CategoryData.Update(categoyId, categoryName);
    }
}
