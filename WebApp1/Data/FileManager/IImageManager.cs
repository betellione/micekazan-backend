namespace WebApp1.Data.FileManager;

public interface IImageManager
{
    public Task<string?> SaveImage(Stream image, string imageName, ImageSizeOptions? options = null);
    public Task<bool> UpdateImage(string imageName, Stream image, ImageSizeOptions? options = null);
    public bool DeleteImage(string imageName);
}