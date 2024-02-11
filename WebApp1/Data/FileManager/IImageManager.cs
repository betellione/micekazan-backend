namespace WebApp1.Data.FileManager;

public interface IImageManager
{
    Task<string?> SaveImage(Stream image, string imageName, ImageSizeOptions? options = null);
    Task<bool> UpdateImage(string imageName, Stream image, ImageSizeOptions? options = null);
    bool DeleteImage(string imageName);
}