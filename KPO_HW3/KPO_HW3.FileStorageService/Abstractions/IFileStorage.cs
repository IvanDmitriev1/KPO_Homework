using DotNext;

namespace KPO_HW3.FileStorageService.Abstractions;

public interface IFileStorage
{
    Task SaveAsync(
        Stream content,
        Guid fileId,
        string contentType,
        CancellationToken ct = default);

    Task<Result<StorageFileInfo>> GetFileInfoAsync(
        Guid fileId,
        CancellationToken ct = default);

    Task GetAsync(
        Guid fileId,
        Stream destination,
        CancellationToken ct = default);

    Task DeleteAsync(
        Guid fileId,
        CancellationToken ct = default);
}