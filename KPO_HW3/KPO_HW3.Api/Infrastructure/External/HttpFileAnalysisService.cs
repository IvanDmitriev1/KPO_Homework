using System.Net;
using System.Text.Json;
using DotNext;
using KPO_HW3.Api.Infrastructure.Exceptions;
using KPO_HW3.Api.Infrastructure.Extensions;

namespace KPO_HW3.Api.Infrastructure.External;

public sealed class HttpFileAnalysisService(HttpClient client) : IFileAnalysisService
{
    public Task<Result<PlagiarismReportSnapshot>> AnalyzeWorkAsync(
        Guid workId,
        CancellationToken cancellationToken = default) =>
        client.PostAsync($"/works/{workId}/analyze", content: null, cancellationToken)
            .ToJsonResultAsync<PlagiarismReportSnapshot>(static (code, body) =>
            {
                if (code == HttpStatusCode.NotFound)
                    return new WorkNotFoundException(body);

                return new DownstreamHttpException(code, body);
            }, cancellationToken);

    public async Task<IReadOnlyList<PlagiarismReportSnapshot>> GetReportsByWorkAsync(
        Guid workId,
        CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync($"/works/{workId}/reports", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return [];
        }

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var reports = await JsonSerializer.DeserializeAsync<List<PlagiarismReportSnapshot>>(stream, cancellationToken: cancellationToken);

        return reports ?? [];
    }
}
