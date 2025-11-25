
using Business_Layer.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Business_Layer.Business;

public class ImagesBusiness : IImagesBusiness
{
    public bool IsAnimatedWebP(Stream webpStream)
    {
        byte[] buffer = new byte[webpStream.Length];
        webpStream.Read(buffer, 0, buffer.Length);

        string content = System.Text.Encoding.ASCII.GetString(buffer);

        return content.Contains("ANIM");
    }
    public async Task StreamImage(string filePath, IFormFile file)
    {
        if (filePath == null || filePath.Length < 1)
        {
            return;
        }
        if (file == null || file.Length < 1)
        {
            return;
        }

        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);
        }
        catch (Exception)
        {

        }
    }
    public async Task<bool> IsValidImage(IFormFile file)
    {
        if (file == null || file.Length < 1)
        {
            return false;
        }

        long maxFileSizeBytes = 3 * 1024 * 1024;
        var allowedExtensions = new[] { ".webp" };
        var allowedContentTypes = new[] { "image/webp" };
        byte[] webpMagicNumber = new byte[] { 0x52, 0x49, 0x46, 0x46 };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        byte[] buffer = new byte[webpMagicNumber.Length];
        using var stream = file.OpenReadStream();
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);


        if (!allowedExtensions.Contains(extension))
            return false;

        if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
            return false;

        if (file.Length > maxFileSizeBytes)
            return false;

        if (bytesRead != webpMagicNumber.Length)
            return false;

        if (!buffer.SequenceEqual(webpMagicNumber))
            return false;

        if (IsAnimatedWebP(stream))
            return false;

        return true;
    }

}
