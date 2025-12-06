using KPO_HW3.FileStorageService.Models;

namespace KPO_HW3.FileStorageService.Abstractions;

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