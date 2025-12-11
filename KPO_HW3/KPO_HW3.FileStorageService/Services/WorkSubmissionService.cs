using DotNext;
using KPO_HW3.FileStorageService.Infrastructure.Data;
using KPO_HW3.FileStorageService.Infrastructure.Data.Entities;
using KPO_HW3.FileStorageService.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW3.FileStorageService.Services;

public class WorkSubmissionService(
    IFileStorage fileStorage,
    FileStorageDbContext dbContext) : IWorkSubmissionService
{
    public async Task<Result<WorkDto>> UploadAsync(Guid studentId, Guid assignmentId, IFormFile file, CancellationToken ct = default)
    {
        await using var fileStream = file.OpenReadStream();

        bool exists =
            await dbContext.Works.AnyAsync(w => w.StudentId == studentId && w.AssignmentId == assignmentId, ct);

        if (exists)
            return new Result<WorkDto>(new WorkAlreadyExistsException(studentId, assignmentId));

        var work = new Work
        {
            StudentId = studentId,
            AssignmentId = assignmentId,
            OriginalFileName = file.FileName
        };

        dbContext.Works.Add(work);
        await dbContext.SaveChangesAsync(ct);

        await fileStorage.SaveAsync(fileStream, work.FileId, file.ContentType, ct);

        return work.ToDto();
    }
}