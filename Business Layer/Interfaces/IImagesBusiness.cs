using Microsoft.AspNetCore.Http;

namespace Business_Layer.Interfaces;

public interface IImagesBusiness
{
    public bool IsAnimatedWebP(Stream webpStream);

    public Task StreamImage(string filePath, IFormFile file);
    public Task<bool> IsValidImage(IFormFile file);
}
