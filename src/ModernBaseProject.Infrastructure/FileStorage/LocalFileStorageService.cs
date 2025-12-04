using ModernBaseProject.Core.Interfaces;

namespace ModernBaseProject.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadPath;

    public LocalFileStorageService()
    {
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_uploadPath, uniqueFileName);

        using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamOutput);

        return uniqueFileName;
    }

    public Task DeleteAsync(string filePath)
    {
        var fullPath = Path.Combine(_uploadPath, filePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public Task<string> GetUrlAsync(string filePath)
    {
        return Task.FromResult($"/uploads/{filePath}");
    }
}
