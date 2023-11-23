namespace Micekazan.PrintDispatcher.Data.FileManager;

public interface IFileManager
{
    public Task<string?> SaveFile(Stream file, string fileName);
    public Stream? ReadFile(string fileName);
    public bool DeleteFile(string fileName);
    public Task<bool> UpdateFile(string fileName, Stream file);
}