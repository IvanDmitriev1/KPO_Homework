using FluentValidation;
using KPO_HW3.FileStorageService.Extensions;
using KPO_HW3.FileStorageService.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Threading;

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

    [ProducesResponseType<Work>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
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

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<FileStreamHttpResult, NotFound, BadRequest<ProblemDetails>>> GetWorkContent(
        HttpContext httpContext,
        [FromServices] FileStorageDbContext dbContext,
        [FromServices] IFileStorage fileStorage,
        [Description("The work id")] Guid id)
    {
        var work = await dbContext.Works.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);
        if (work is null)
        {
            return TypedResults.NotFound();
        }

        var stream = fileStorage.GetAsync(work.FilePath, httpContext.RequestAborted);
        if (stream is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.File(stream, "application/octet-stream", Path.GetFileName(work.FilePath));
    }

    [ProducesResponseType<Work>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Created<Work>, BadRequest<ProblemDetails>>> PostUploadWork(
    HttpContext httpContext,
    [FromServices] FileStorageDbContext dbContext,
    [FromServices] IFileStorage fileStorage,
    [FromServices] IValidator<Work> validator,
    [FromForm(Name = nameof(studentId)), Description("Student id")] Guid studentId,
    [FromForm(Name = nameof(assignmentId)), Description("Assignment id")] Guid assignmentId,
    [FromForm(Name = nameof(file))] IFormFile file,
    CancellationToken ct)
    {
        if (file.Length > 20 * 1024 * 1024) // 20MB
        {
            var problem = httpContext.CreateProblem(StatusCodes.Status400BadRequest, "File is too large.");
            return TypedResults.BadRequest(problem);
        }

        bool exists =
            await dbContext.Works.AnyAsync(w =>
                w.StudentId == studentId || w.AssignmentId == assignmentId, ct);
        if (exists)
        {
            var problem = httpContext.CreateProblem(StatusCodes.Status400BadRequest, "Work already exists");
            return TypedResults.BadRequest(problem);
        }

        await using var fileStream = file.OpenReadStream();
        var storedFilePath = await fileStorage.SaveAsync(fileStream, ct);

        var work = new Work
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            AssignmentId = assignmentId,
            FilePath = storedFilePath,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var validationResult = validator.Validate(work);
        if (!validationResult.IsValid)
        {
            await fileStorage.DeleteAsync(storedFilePath, ct);

            var problem = validationResult.ToProblemDetails(httpContext);
            return TypedResults.BadRequest(problem);
        }

        dbContext.Works.Add(work);
        await dbContext.SaveChangesAsync(ct);

        var location = $"works/{work.Id}";
        return TypedResults.Created(location, work);
    }
}