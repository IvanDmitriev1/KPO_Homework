namespace KPO_HW3.Api.Abstractions;

public interface IFileStorageService
{
    Task<WorkSnapshot?> GetWorkAsync(
        Guid workId,
        CancellationToken cancellationToken = default);

    Task<WorkSnapshot> UploadWorkAsync(
        Guid studentId,
        Guid assignmentId,
        IFormFile file,
        CancellationToken cancellationToken = default);

    Task<HttpResponseMessage?> GetWorkContentResponseAsync(
        Guid workId,
        CancellationToken cancellationToken = default);
}