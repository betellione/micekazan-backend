using Serilog;
using ILogger = Serilog.ILogger;

namespace WebApp1.Data.FileManager;

public class ImageManager : IImageManager
{
    private readonly IFileManager _fileManager;
    private readonly ILogger _logger = Log.ForContext<IImageManager>();

    public ImageManager(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public async Task<string?> SaveImage(Stream image, string imageName, ImageSizeOptions? options = null)
    {
        return options is null
            ? await _fileManager.SaveFile(image, imageName)
            : await SaveImageWithResize(image, imageName, options);
    }

    public async Task<bool> UpdateImage(string imageName, Stream image, ImageSizeOptions? options = null)
    {
        return options is null
            ? await _fileManager.UpdateFile(imageName, image)
            : await SaveImageWithResize(image, imageName, options, false) is not null;
    }

    public bool DeleteImage(string imageName)
    {
        return _fileManager.DeleteFile(imageName);
    }

    private async Task<string?> SaveImageWithResize(Stream stream, string imageName, ImageSizeOptions options, bool saveAsNew = true)
    {
        var ext = Path.GetExtension(imageName);
        var savePath = saveAsNew ? _fileManager.GeneratePathToSave(ext) : _fileManager.GetFullPath(imageName);

        try
        {
            using var image = await Image.LoadAsync(stream);

            image.Mutate(x =>
            {
                x.Resize(new ResizeOptions
                {
                    Size = new Size(options.Width, options.Height),
                    Mode = ResizeMode.Crop,
                    Position = AnchorPositionMode.Center,
                });
            });

            await image.SaveAsync(savePath);

            return _fileManager.GetRelativePath(savePath);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Cannot resize and save image to path {Path} with width options {Options}", savePath, options);
            return null;
        }
    }
}