using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.AspNetCore.Authorization;
using Business_Layer.Interfaces;
using Presentation_Layer.Authorization;
using Enums;

namespace Presentation_Layer.Controllers;

[ApiController]
[Route("[controller]")]

public class CategoriesController : ControllerBase
{
    public ICategoryBusiness CategoryBusiness { get; }

    public CategoriesController(ICategoryBusiness categoryBusiness)
    {
        CategoryBusiness = categoryBusiness;
    }



    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<Category>>> GetCategories()
    {
        var result = await CategoryBusiness.GetAll();
        return result != null ?
            Ok(result) : NotFound();
    }



    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await CategoryBusiness.GetById(id);

        return category != null ?
         Ok(category) : NotFound();
    }



    [Authorize]
    [CheckPermission(Permission.Categories_ManageCategories)]
    [HttpPost("{name}")]
    public async Task<ActionResult<Category>> CreateCategory(string name)
    {
        return await CategoryBusiness.Add(name) ?
         Created() : BadRequest();
    }


    [Authorize]
    [CheckPermission(Permission.Categories_ManageCategories)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromQuery] string name)
    {
        return await CategoryBusiness.Update(id, name) ?
          NoContent() : BadRequest();
    }


    [Authorize]
    [CheckPermission(Permission.Categories_ManageCategories)]
    [HttpPatch("{categoryId}")]
    public async Task<IActionResult> AddImage(int categoryId, IFormFile image)
    {
        try
        {
            return await CategoryBusiness.AddImage(categoryId, image) ?
                 Ok() : NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }


}