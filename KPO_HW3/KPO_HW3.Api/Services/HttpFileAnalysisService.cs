using System.Net;
using System.Text.Json;

namespace KPO_HW3.Api.Services;

public sealed class HttpFileAnalysisService(HttpClient client) : IFileAnalysisService
{
    public async Task<PlagiarismReportSnapshot> AnalyzeWorkAsync(
        Guid workId,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsync($"/works/{workId}/analyze", content: null, cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var report = await JsonSerializer.DeserializeAsync<PlagiarismReportSnapshot>(stream, cancellationToken: cancellationToken);

        if (report is null)
        {
            throw new InvalidOperationException("FileAnalysisService returned empty report.");
        }

        return report;
    }

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
