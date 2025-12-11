using KPO_HW3.Api.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Runtime.ExceptionServices;

namespace KPO_HW3.Api.Endpoints;

public sealed record WorkApiServices(
    IFileStorageService FileStorage,
    IFileAnalysisService FileAnalysis);

public static class WorkEndpoints
{
    public static RouteGroupBuilder MapWorkEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/works")
            .WithTags("Works");

        group.MapPost("/upload", UploadWork)
            .WithName("UploadWork")
            .WithSummary("Upload student work")
            .WithDescription("Uploads a student work, stores it and triggers plagiarism analysis.")
            .DisableAntiforgery();

        group.MapGet("/{id:guid}", GetWorkById)
            .WithName("GetWorkById")
            .WithSummary("Get work by id")
            .WithDescription("Returns stored work metadata by its identifier.");

        group.MapGet("/{id:guid}/reports", GetWorkReports)
            .WithName("GetWorkReports")
            .WithSummary("Get plagiarism reports for a work")
            .WithDescription("Returns plagiarism reports for the specified work.");

        group.MapGet("/{id:guid}/content", GetWorkContent)
            .WithName("GetWorkContent")
            .WithSummary("Get work content")
            .WithDescription("Returns the stored file content for a work.");

        return group;
    }

    public static async Task<Results<Ok<WorkSnapshot>, NotFound, Conflict>> UploadWork(
        [FromServices] WorkApiServices services,
        [Description("Student id")] Guid studentId,
        [Description("Assignment id")] Guid assignmentId,
        [FromForm(Name = "file")] IFormFile file,
        CancellationToken ct)
    {
        var snapshotResult = await services.FileStorage.UploadWorkAsync(
            studentId,
            assignmentId,
            file,
            ct);

        if (snapshotResult.TryGet(out var snapshot))
        {
            _ = services.FileAnalysis.AnalyzeWorkAsync(snapshot.Id, ct);
            return TypedResults.Ok(snapshot);
        }

        switch (snapshotResult.Error)
        {
            case WorkNotFoundException:
                return TypedResults.NotFound();
            case WorkAlreadyExistsException:
                return TypedResults.Conflict();
        }

        ExceptionDispatchInfo.Throw(snapshotResult.Error!);
        throw new InvalidOperationException();
    }

    public static async Task<Results<Ok<WorkSnapshot>, NotFound, Conflict>> GetWorkById(
        [FromServices] WorkApiServices services,
        [Description("The work id")] Guid id,
        CancellationToken ct)
    {
        var snapshotResult = await services.FileStorage.GetWorkAsync(id, ct);
        if (snapshotResult.TryGet(out var snapshot))
        {
            return TypedResults.Ok(snapshot);
        }

        switch (snapshotResult.Error)
        {
            case WorkNotFoundException:
                return TypedResults.NotFound();
            case WorkAlreadyExistsException:
                return TypedResults.Conflict();
        }

        ExceptionDispatchInfo.Throw(snapshotResult.Error!);
        throw new InvalidOperationException();
    }

    public static async Task<Results<Ok<PlagiarismReportSnapshot>, NotFound>> GetWorkReports(
        [FromServices] WorkApiServices services,
        [Description("The work id")] Guid id,
        CancellationToken ct)
    {
        var reportResult = await services.FileAnalysis.GetReportsByWorkAsync(id, ct);
        if (reportResult.TryGet(out var report))
        {
            return TypedResults.Ok(report);
        }

        switch (reportResult.Error)
        {
            case PlagiarismReportNotFoundException:
                return TypedResults.NotFound();
        }

        ExceptionDispatchInfo.Throw(reportResult.Error!);
        throw new InvalidOperationException();
    }

    
    public static async Task<Results<PushStreamHttpResult, NotFound, Conflict>> GetWorkContent(
        [FromServices] WorkApiServices services,
        [Description("The work id")] Guid id,
        CancellationToken ct)
    {
        var contentResponseResult = await services.FileStorage.GetWorkContentResponseAsync(id, ct);
        if (!contentResponseResult.TryGet(out var contentResponse))
        {
            switch (contentResponseResult.Error)
            {
                case WorkNotFoundException:
                    return TypedResults.NotFound();
                case WorkAlreadyExistsException:
                    return TypedResults.Conflict();
            }

            ExceptionDispatchInfo.Throw(contentResponseResult.Error!);
            throw new InvalidOperationException();
        }

        string contentType = contentResponse.Content.Headers.ContentType?.ToString() ?? throw new InvalidOperationException();
        var cd = contentResponse.Content.Headers.ContentDisposition;
        var fileName = cd?.FileNameStar ?? cd?.FileName;

        var result = TypedResults.Stream(
            async responseStream =>
            {
                using var _ = contentResponse;
                await using var sourceStream = await contentResponse.Content.ReadAsStreamAsync(ct);
                await sourceStream.CopyToAsync(responseStream, ct);
            },
            contentType: contentType,
            fileDownloadName: fileName);

        return result;
    }
}
