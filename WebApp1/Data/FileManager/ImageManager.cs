using Serilog;
using ILogger = Serilog.ILogger;

namespace WebApp1.Data.FileManager;

public class ImageManager(IFileManager fileManager) : IImageManager
{
    private readonly ILogger _logger = Log.ForContext<IImageManager>();

    public async Task<string?> SaveImage(Stream image, string imageName, ImageSizeOptions? options = null)
    {
        return options is null
            ? await fileManager.SaveFile(image, imageName)
            : await SaveImageWithResize(image, imageName, options.Width, options.Height);
    }

    public async Task<bool> UpdateImage(string path, Stream image, ImageSizeOptions? options = null)
    {
        var fileName = Path.GetFileName(path);

        return options is null
            ? await fileManager.UpdateFile(fileName, image)
            : await SaveImageWithResize(image, path, options.Width, options.Height, path) is not null;
    }

    private async Task<string?> SaveImageWithResize(Stream stream, string imageName, int width, int height, string? path = null)
    {
        var ext = Path.GetExtension(imageName);
        path ??= fileManager.GeneratePathToSave(ext);

        try
        {
            using var image = await Image.LoadAsync(stream);

            image.Mutate(x =>
            {
                x.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Max,
                });
            });

            await image.SaveAsync(path);

            return path;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Cannot resize and save image to path {Path} with width {Width} and height {Height}", path, width, height);
            return null;
        }
    }
}