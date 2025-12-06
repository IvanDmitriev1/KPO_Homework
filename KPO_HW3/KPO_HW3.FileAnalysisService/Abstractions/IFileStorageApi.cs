namespace KPO_HW3.FileAnalysisService.Abstractions;


public interface IFileStorageApi
{
    Task<WorkSnapshot?> GetWorkAsync(
        Guid workId,
        CancellationToken ct = default);

    Task<Stream> GetWorkContentAsync(
        Guid workId,
        CancellationToken ct = default);
}