using KPO_HW3.FileAnalysisService.Infrastructure.External;
using System.Net;

namespace KPO_HW3.FileAnalysisService.Services;

public class HttpFileStorageApi(HttpClient client) : IFileStorageApi
{
    public async Task<WorkSnapshot?> GetWorkAsync(
        Guid workId,
        CancellationToken ct = default)
    {
        using var response = await client.GetAsync($"/works/{workId}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var snapshot = await response.Content.ReadFromJsonAsync<WorkSnapshot>(
            cancellationToken: ct);

        return snapshot;
    }
    public async Task<Stream> GetWorkContentAsync(Guid workId, CancellationToken ct = default)
    {
        var response = await client.GetAsync(
            $"/works/{workId}/content",
            HttpCompletionOption.ResponseHeadersRead,
            ct);

        response.EnsureSuccessStatusCode();

        var stream = await HttpResponseContentStream.Create(response);
        return stream;
    }
}