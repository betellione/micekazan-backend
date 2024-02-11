namespace WebApp1.Data.FileManager;

public interface IFileManager
{
    Task<string?> SaveFile(Stream file, string fileName);
    Stream? ReadFile(string fileName);
    bool DeleteFile(string fileName);
    Task<bool> UpdateFile(string fileName, Stream file);
    string GeneratePathToSave(string fileExtension);
    string GetFullPath(string fileName);
    string GetRelativePath(string path);
}