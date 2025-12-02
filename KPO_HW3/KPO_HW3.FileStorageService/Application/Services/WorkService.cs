using DotNext;
using FluentValidation;
using KPO_HW3.FileStorageService.Domain.Exceptions;
using KPO_HW3.FileStorageService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW3.FileStorageService.Application.Services;

public class WorkService(
    IFileStorage fileStorage,
    FileStorageDbContext dbContext) : IWorkService
{
    public async Task<Result<Work>> UploadAsync(
        Guid studentId,
        Guid assignmentId,
        string fileExtension,
        Stream fileContent,
        CancellationToken ct = default)
    {
        bool exists =
            await dbContext.Works.AnyAsync(w =>
                w.StudentId == studentId && w.AssignmentId == assignmentId, ct);
        if (exists)
            return new Result<Work>(new WorkAlreadyExistsException(studentId, assignmentId));

        var storedFilePath = await fileStorage.SaveAsync(fileContent, fileExtension, ct);

        var work = new Work
        {
            StudentId = studentId,
            AssignmentId = assignmentId,
            FilePath = storedFilePath,
        };

        dbContext.Works.Add(work);
        await dbContext.SaveChangesAsync(ct);

        return work;
    }
}