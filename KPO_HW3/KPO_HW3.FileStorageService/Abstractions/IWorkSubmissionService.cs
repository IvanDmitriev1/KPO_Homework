using DotNext;

namespace KPO_HW3.FileStorageService.Abstractions;

public interface IWorkSubmissionService
{
    Task<Result<WorkDto>> UploadAsync(
        Guid studentId,
        Guid assignmentId,
        IFormFile file,
        CancellationToken ct = default);
}