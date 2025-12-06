using DotNext;
using KPO_HW3.FileStorageService.Infrastructure.Data;
using KPO_HW3.FileStorageService.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW3.FileStorageService.Services;

public class WorkService(
    IFileStorage fileStorage,
    FileStorageDbContext dbContext) : IWorkService
{
    public async Task<Result<Work>> UploadAsync(Guid studentId, Guid assignmentId, IFormFile file, CancellationToken ct = default)
    {
        await using var fileStream = file.OpenReadStream();

        bool exists =
            await dbContext.Works.AnyAsync(w => w.StudentId == studentId && w.AssignmentId == assignmentId, ct);

        if (exists)
            return new Result<Work>(new WorkAlreadyExistsException(studentId, assignmentId));

        var fileId = await fileStorage.SaveAsync(fileStream, file.FileName, file.ContentType, ct);

        var work = new Work
        {
            StudentId = studentId,
            AssignmentId = assignmentId,
            FileId = fileId
        };

        dbContext.Works.Add(work);
        await dbContext.SaveChangesAsync(ct);

        return work;
    }
}