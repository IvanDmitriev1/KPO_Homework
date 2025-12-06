using DotNext;
using KPO_HW3.FileStorageService.Models;

namespace KPO_HW3.FileStorageService.Abstractions;

public interface IWorkService
{
    Task<Result<Work>> UploadAsync(
        Guid studentId,
        Guid assignmentId,
        IFormFile file,
        CancellationToken ct = default);
}