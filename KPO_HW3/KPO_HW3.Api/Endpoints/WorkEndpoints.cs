using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

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
    public static async Task<Results<Ok<WorkSnapshot>, BadRequest<ProblemDetails>>> UploadWork(
        [FromServices] WorkApiServices services,
        [Description("Student id")] Guid studentId,
        [Description("Assignment id")] Guid assignmentId,
        [FromForm(Name = "file")] IFormFile file,
        CancellationToken ct)
    {
        var snapshot = await services.FileStorage.UploadWorkAsync(
            studentId,
            assignmentId,
            file,
            ct);

        _ = services.FileAnalysis.AnalyzeWorkAsync(snapshot.WorkId, ct);

        return TypedResults.Ok(snapshot);
    }

    [ProducesResponseType<WorkSnapshot>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<WorkSnapshot>, NotFound, BadRequest<ProblemDetails>>> GetWorkById(
        [FromServices] WorkApiServices services,
        [Description("The work id")] Guid id,
        CancellationToken ct)
    {
        var snapshot = await services.FileStorage.GetWorkAsync(id, ct);
        if (snapshot is null)
        {
            return TypedResults.NotFound();
        }

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
    public static async Task<Results<PushStreamHttpResult, NotFound, BadRequest<ProblemDetails>>> GetWorkContent(
        [FromServices] WorkApiServices services,
        [Description("The work id")] Guid id,
        CancellationToken ct)
    {

        var response = await services.FileStorage.GetWorkContentResponseAsync(id, ct);
        if (response is null)
        {
            return TypedResults.NotFound();
        }

        string contentType = response.Content.Headers.ContentType?.ToString() ?? throw new InvalidOperationException();
        var cd = response.Content.Headers.ContentDisposition;
        var fileName = cd?.FileNameStar ?? cd?.FileName;

        var result = TypedResults.Stream(
            async responseStream =>
            {
                using (response);
                await using var sourceStream = await response.Content.ReadAsStreamAsync(ct);
                await sourceStream.CopyToAsync(responseStream, ct);
            },
            contentType: contentType,
            fileDownloadName: fileName);

        return result;
    }
}
