using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using KPO_HW3.Api.Infrastructure.Extensions;

namespace KPO_HW3.Api.Endpoints;

public static class WordCloudEndpoints
{
    public static RouteGroupBuilder MapWordCloudEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/works")
            .WithTags("Analysis");

        group.MapGet("/{id:guid}/wordcloud", GetWorkWordCloud)
            .WithName("GetWorkWordCloud")
            .WithSummary("Get word cloud for work")
            .WithDescription("Returns a word cloud visualization of the specified work as an image.");

        return group;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<IResult> GetWorkWordCloud(
        IWordCloudService wordCloudService,
        [Description("The work id")] Guid id,
        CancellationToken ct)
    {
        var streamResult = await wordCloudService.GenerateImageForWorkAsync(id, ct);
        if (!streamResult.TryGet(out var stream))
        {
            return streamResult.ToHttpResult();
        }

        var result = TypedResults.Stream(
            async output =>
            {
                await using var _ = stream;
                await stream.CopyToAsync(output, ct);
            },
            contentType: "image/png"
        );

        return result;
    }
}
