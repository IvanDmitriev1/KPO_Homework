using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using KPO_HW3.Api.Infrastructure.Extensions;

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

    [ProducesResponseType<WorkSnapshot>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<IResult> UploadWork(
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

        if (!snapshotResult.TryGet(out var snapshot))
            return snapshotResult.ToHttpResult();

        _ = services.FileAnalysis.AnalyzeWorkAsync(snapshot.WorkId, ct);

        return TypedResults.Ok(snapshot);
    }

    [ProducesResponseType<WorkSnapshot>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<IResult> GetWorkById(
        [FromServices] WorkApiServices services,
        [Description("The work id")] Guid id,
        CancellationToken ct)
    {
        var snapshotResult = await services.FileStorage.GetWorkAsync(id, ct);
        if (!snapshotResult.TryGet(out var snapshot))
            return snapshotResult.ToHttpResult();

        return TypedResults.Ok(snapshot);
    }

    [ProducesResponseType<IEnumerable<PlagiarismReportSnapshot>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<IReadOnlyList<PlagiarismReportSnapshot>>, BadRequest<ProblemDetails>>> GetWorkReports(
        [FromServices] WorkApiServices services,
        [Description("The work id")] Guid id,
        CancellationToken ct)
    {
        var reports = await services.FileAnalysis.GetReportsByWorkAsync(id, ct);
        return TypedResults.Ok(reports);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<IResult> GetWorkContent(
        [FromServices] WorkApiServices services,
        [Description("The work id")] Guid id,
        CancellationToken ct)
    {
        var contentResponseResult = await services.FileStorage.GetWorkContentResponseAsync(id, ct);
        if (!contentResponseResult.TryGet(out var contentResponse))
        {
            return contentResponseResult.ToHttpResult();
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
