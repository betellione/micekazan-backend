using Serilog;
using ILogger = Serilog.ILogger;

namespace WebApp1.Data.FileManager;

public class FileManager : IFileManager
{
    private readonly string _basePath;
    private readonly string _fullBasePath;
    private readonly ILogger _logger = Log.ForContext<IFileManager>();

    public FileManager(string basePath, string skewPath)
    {
        _basePath = basePath;
        _fullBasePath = Path.Combine(_basePath, skewPath);
        if (!Directory.Exists(_fullBasePath)) Directory.CreateDirectory(_fullBasePath);
    }

    public async Task<string?> SaveFile(Stream file, string fileName)
    {
        var savePath = GeneratePathToSave(Path.GetExtension(fileName));

        try
        {
            await using var fileStream = new FileStream(savePath, FileMode.Create);
            await file.CopyToAsync(fileStream);
            return Path.GetRelativePath(_basePath, savePath);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Cannot save file {OriginFileName} with name {SavePath}", fileName, savePath);
            return null;
        }
    }

    public Stream? ReadFile(string fileName)
    {
        var path = Path.Combine(_basePath, fileName);

        try
        {
            var stream = File.OpenRead(path);
            return stream;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Cannot read file with path {Path}", path);
            return null;
        }
    }

    public bool DeleteFile(string fileName)
    {
        var path = Path.Combine(_fullBasePath, fileName);

        try
        {
            File.Delete(path);
            return true;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Cannot delete file with path {Path}", path);
            return false;
        }
    }

    public async Task<bool> UpdateFile(string fileName, Stream file)
    {
        var path = Path.Combine(_fullBasePath, fileName);

        try
        {
            await using var stream = new FileStream(path, FileMode.Truncate);
            await file.CopyToAsync(stream);
            return true;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Cannot update file with path {Path}", path);
            return false;
        }
    }

    public string GeneratePathToSave(string fileExtension)
    {
        var fileName = Guid.NewGuid() + fileExtension;
        return Path.Combine(_fullBasePath, fileName);
    }
}