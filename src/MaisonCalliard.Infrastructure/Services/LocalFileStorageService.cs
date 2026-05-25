using MaisonCalliard.Application.Files;

namespace MaisonCalliard.Infrastructure.Services;

// TODO: Replace with a real blob storage implementation (e.g. Azure Blob Storage).
internal sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadPath;
    private readonly string _baseUrl;

    public LocalFileStorageService(string uploadPath, string baseUrl)
    {
        _uploadPath = uploadPath;
        _baseUrl = baseUrl;
    }

    public async Task<string> SaveAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadPath, uniqueName);

        Directory.CreateDirectory(_uploadPath);

        await using var destination = File.Create(filePath);
        await fileStream.CopyToAsync(destination, cancellationToken);

        return $"{_baseUrl.TrimEnd('/')}/uploads/{uniqueName}";
    }

    public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
        var filePath = Path.Combine(_uploadPath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
