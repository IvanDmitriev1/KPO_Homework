using DotNext;

namespace KPO_HW3.FileStorageService.Domain.Interfaces;

public interface IWorkService
{
    Task<Result<Work>> UploadAsync(
        Guid studentId,
        Guid assignmentId,
        string fileExtension,
        Stream fileContent,
        CancellationToken ct = default);
}