namespace WebApp1.Data.FileManager;

public interface IImageManager
{
    public Task<string?> SaveImage(Stream image, string imageName, ImageSizeOptions? options = null);
    public Task<bool> UpdateImage(string path, Stream image, ImageSizeOptions? options = null);
}