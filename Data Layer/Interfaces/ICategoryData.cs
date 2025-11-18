using Microsoft.Data.SqlClient;
using System.Data;
using Models;

namespace Data_Layer.Interfaces;

public interface ICategoryData
{
    Task<List<Category>> GetAll();
    Task<Category> GetById(int id);
    Task<bool> Add(string name);
    Task<bool> Update(int categoyId,string categoryName);
    Task SetCategoryImage(string filePath, int categoryId);
}
