using KPO_HW3.FileStorageService.Infrastructure.Data;
using KPO_HW3.FileStorageService.Infrastructure.Data.Entities;
using KPO_HW3.FileStorageService.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Runtime.ExceptionServices;

namespace KPO_HW3.FileStorageService.Endpoints;

public static class WorkEndpoints
{
    public static IEndpointRouteBuilder MapWorkEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/works")
            .WithTags("Works");

        group.MapPost("/upload", PostUploadWork)
            .WithName("UploadWork")
            .WithSummary("Upload student work")
            .WithDescription("Uploads a student work and stores its metadata.")
            .DisableAntiforgery();

        group.MapGet("/{id:guid}", GetWorkById)
            .WithName("GetWorkById")
            .WithSummary("Get work by id")
            .WithDescription("Returns stored work metadata by its identifier.");

        group.MapGet("/{id:guid}/content", GetWorkContent)
            .WithName("GetWorkContent")
            .WithSummary("Get work content")
            .WithDescription("Returns the stored file content for a work.");

        return endpoints;
    }

    public static async Task<Results<Ok<Work>, NotFound, BadRequest<ProblemDetails>>> GetWorkById(
        HttpContext httpContext,
        [FromServices] FileStorageDbContext dbContext,
        [FromRoute, Description("The work id")] Guid id)
    {
        var work = await dbContext.Works.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);
        if (work is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(work);
    }

    public static async Task<Results<PushStreamHttpResult, NotFound, Conflict>> GetWorkContent(
        [FromServices] FileStorageDbContext dbContext,
        [FromServices] IFileStorage fileStorage,
        [Description("The work id")] Guid id,
        CancellationToken ct)
    {
        var work = await dbContext.Works.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id, cancellationToken: ct);
        if (work is null)
            return TypedResults.NotFound();

        var fileInfoResult = await fileStorage.GetFileInfoAsync(work.FileId, ct);
        if (fileInfoResult.TryGet(out var fileInfo))
        {
            var result = TypedResults.Stream(
                async responseStream =>
                {
                    await fileStorage.GetAsync(work.FileId, responseStream, ct);
                },
                contentType: fileInfo.ContentType,
                fileDownloadName: work.OriginalFileName
            );

            return result;
        }

        switch (fileInfoResult.Error)
        {
            case FileNotFoundException:
                return TypedResults.NotFound();
            case WorkAlreadyExistsException:
                return TypedResults.Conflict();
        }

        ExceptionDispatchInfo.Throw(fileInfoResult.Error!);
        throw new InvalidOperationException("Unreachable code.");
    }

    public static async Task<Results<Created<WorkDto>, NotFound, Conflict, BadRequest<ProblemDetails>>> PostUploadWork(
    HttpContext httpContext,
    [FromServices] IWorkSubmissionService workSubmissionService,
    [FromForm(Name = nameof(studentId)), Description("Student id")] Guid studentId,
    [FromForm(Name = nameof(assignmentId)), Description("Assignment id")] Guid assignmentId,
    [FromForm(Name = nameof(file))] IFormFile file,
    CancellationToken ct)
    {
        if (file.Length == 0)
        {
            return TypedResults.BadRequest(new ProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "File is empty."
            });
        }

        var result = await workSubmissionService.UploadAsync(studentId, assignmentId, file, ct);
        if (result.TryGet(out var work))
        {
            var location = $"works/{work.Id}";
            return TypedResults.Created(location, work);
        }

        switch (result.Error)
        {
            case FileNotFoundException:
                return TypedResults.NotFound();
            case WorkAlreadyExistsException:
                return TypedResults.Conflict();
        }

        ExceptionDispatchInfo.Throw(result.Error!);
        throw new InvalidOperationException("Unreachable code.");
    }
}