using Microsoft.AspNetCore.Http;
using Models;

namespace Business_Layer.Interfaces;

public interface ICategoryBusiness
{
    Task<List<Category>> GetAll();
    Task<Category> GetById(int id);
    Task<bool> Add(string name);
    Task<bool> Update(int categoyId,string categoryName);
    Task<bool> AddImage(int categoryId, IFormFile formFile);
}
