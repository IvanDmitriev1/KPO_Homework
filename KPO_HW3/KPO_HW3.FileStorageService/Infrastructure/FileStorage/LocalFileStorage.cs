namespace KPO_HW3.FileStorageService.Infrastructure.FileStorage;

public class LocalFileStorage : IFileStorage
{
    public LocalFileStorage(IHostEnvironment hostEnvironment, string? root)
    {
        _root = root is null
            ? Path.Combine(hostEnvironment.ContentRootPath, "data")
            : Path.GetFullPath(root);

        Directory.CreateDirectory(_root);
    }

    private readonly string _root;

    public async Task<string> SaveAsync(Stream content, string extention, CancellationToken cancellationToken = default)
    {
        var newFileName = $"{Guid.NewGuid():N}{extention}";
        var fullPath = Path.Combine(_root, newFileName);

        await using var fileStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await content.CopyToAsync(fileStream, cancellationToken);

        return newFileName;
    }

    public Stream? GetAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_root, filePath);

        if (!File.Exists(fullPath))
            return null;

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return stream;
    }

    public Task DeleteAsync(string storagePath, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_root, storagePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}