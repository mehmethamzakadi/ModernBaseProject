namespace ModernBaseProject.Core.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteAsync(string filePath);
    Task<string> GetUrlAsync(string filePath);
}
