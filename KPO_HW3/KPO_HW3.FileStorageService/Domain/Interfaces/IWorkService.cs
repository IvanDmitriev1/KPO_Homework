using DotNext;

namespace KPO_HW3.FileStorageService.Domain.Interfaces;

public interface IWorkService
{
    Task<Result<Work>> UploadAsync(
        Guid studentId,
        Guid assignmentId,
        IFormFile file,
        CancellationToken ct = default);
}