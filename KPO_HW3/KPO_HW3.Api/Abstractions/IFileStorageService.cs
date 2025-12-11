using DotNext;

namespace KPO_HW3.Api.Abstractions;

public interface IFileStorageService
{
    Task<Result<WorkSnapshot>> GetWorkAsync(
        Guid workId,
        CancellationToken cancellationToken = default);

    Task<Result<WorkSnapshot>> UploadWorkAsync(
        Guid studentId,
        Guid assignmentId,
        IFormFile file,
        CancellationToken cancellationToken = default);

    Task<Result<HttpResponseMessage>> GetWorkContentResponseAsync(
        Guid workId,
        CancellationToken cancellationToken = default);
}