using KPO_HW3.FileStorageService.Infrastructure.FileStorage;

namespace KPO_HW3.FileStorageService.Domain.Interfaces;

public interface IFileStorage
{
    Task<string> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken ct = default);

    Task<StorageFileInfo?> GetFileInfoAsync(
        string fileId,
        CancellationToken ct = default);

    Task GetAsync(
        string fileId,
        Stream destination,
        CancellationToken ct = default);
    Task DeleteAsync(
        string fileId,
        CancellationToken ct = default);
}