using KPO_HW3.FileAnalysisService.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Runtime.ExceptionServices;

namespace KPO_HW3.FileAnalysisService.Endpoints;

public static class ReportEndpoints
{
    public static RouteGroupBuilder MapReportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/works")
            .WithTags("Analysis");

        group.MapPost("/{id:guid}/analyze", AnalyzeWork)
            .WithName("AnalyzeWork")
            .WithSummary("Run plagiarism analysis for a work")
            .WithDescription("Runs plagiarism analysis for the specified work and stores a report.");

        group.MapGet("/{id:guid}/reports", GetReportsByWork)
            .WithName("GetReportsByWork")
            .WithSummary("Get plagiarism reports for a work")
            .WithDescription("Returns all plagiarism reports for the specified work.");

        return group;
    }

    public static async Task<Results<Ok<PlagiarismReportDto>, NotFound, BadRequest<ProblemDetails>>> AnalyzeWork(
        [FromServices] IAnalysisService services,
        [FromRoute, Description("The work id")] Guid id,
        CancellationToken ct)
    {
        var result = await services.AnalyzeAsync(id, ct);
        if (result.TryGet(out var value))
        {
            return TypedResults.Ok(value);
        }

        switch (result.Error)
        {
            case WorkNotFoundException:
                return TypedResults.NotFound();
        }

        ExceptionDispatchInfo.Throw(result.Error!);
        throw new InvalidOperationException();
    }

    public static async Task<Results<Ok<PlagiarismReportDto>, NotFound, BadRequest<ProblemDetails>>> GetReportsByWork(
        [FromServices] AnalysisDbContext dbContext,
        [FromRoute, Description("The work id")] Guid id,
        CancellationToken ct)
    {
        var item = await dbContext.PlagiarismReports
            .AsNoTracking()
            .Include(r => r.Matches)
            .FirstOrDefaultAsync(r => r.WorkId == id, cancellationToken: ct);

        if (item is not null)
        {
            return TypedResults.Ok(item.ToDto());
        }

        return TypedResults.NotFound();
    }
}