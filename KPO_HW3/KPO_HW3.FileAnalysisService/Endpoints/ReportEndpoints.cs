using DotNext;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using KPO_HW3.FileAnalysisService.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using KPO_HW3.FileAnalysisService.Infrastructure.Extensions;

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

    [ProducesResponseType<PlagiarismReport>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<IResult> AnalyzeWork(
        HttpContext httpContext,
        [FromServices] IAnalysisService services,
        [FromRoute, Description("The work id")] Guid id,
        CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            var problem = httpContext.CreateProblem(
                StatusCodes.Status400BadRequest,
                "Invalid work id.");

            return TypedResults.BadRequest(problem);
        }

        var result = await services.AnalyzeAsync(id, ct);

        return result.IsSuccessful ? TypedResults.Ok(result.Value) : result.ToHttpResult(httpContext);
    }


    [ProducesResponseType<IEnumerable<PlagiarismReport>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<List<PlagiarismReport>>, BadRequest<ProblemDetails>>> GetReportsByWork(
        HttpContext httpContext,
        [FromServices] AnalysisDbContext dbContext,
        [FromRoute, Description("The work id")] Guid id,
        CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            var problem = httpContext.CreateProblem(
                StatusCodes.Status400BadRequest,
                "Invalid work id.");

            return TypedResults.BadRequest(problem);
        }

        var items = await dbContext.PlagiarismReports
            .AsNoTracking()
            .Where(r => r.WorkId == id)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return TypedResults.Ok(items);
    }
}