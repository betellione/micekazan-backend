namespace Micekazan.PrintDispatcher.Data.FileManager;

public class FileManager : IFileManager
{
    private readonly string _fullBasePath;

    public FileManager(string basePath, string skewPath)
    {
        _fullBasePath = Path.Combine(basePath, skewPath);
        if (!Directory.Exists(_fullBasePath)) Directory.CreateDirectory(_fullBasePath);
    }

    public async Task<string?> SaveFile(Stream file, string fileName)
    {
        var savePath = Path.Combine(_fullBasePath, fileName);

        try
        {
            await using var fileStream = new FileStream(savePath, FileMode.Create);
            await file.CopyToAsync(fileStream);
            return savePath;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public Stream? ReadFile(string fileName)
    {
        var path = Path.Combine(_fullBasePath, fileName);

        try
        {
            var stream = File.OpenRead(path);
            return stream;
        }
        catch (Exception)
        {
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
        catch (Exception)
        {
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
        catch (Exception)
        {
            return false;
        }
    }
}