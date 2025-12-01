namespace KPO_HW3.FileStorageService.Domain.Interfaces;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream content, CancellationToken ct = default);
    Stream? GetAsync(string storagePath, CancellationToken ct = default);
    Task DeleteAsync(string storagePath, CancellationToken ct = default);
}