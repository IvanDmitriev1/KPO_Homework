using System.Net;
using DotNext;
using KPO_HW3.FileAnalysisService.Infrastructure.Exceptions;

namespace KPO_HW3.FileAnalysisService.Services;

public class HttpFileStorageApi(HttpClient client) : IFileStorageApi
{
    public async Task<Result<WorkSnapshot>> GetWorkAsync(
        Guid workId,
        CancellationToken ct = default)
    {
        using var response = await client.GetAsync($"/works/{workId}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Result.FromException<WorkSnapshot>(new WorkNotFoundException(workId));

        response.EnsureSuccessStatusCode();

        var snapshot = await response.Content.ReadFromJsonAsync<WorkSnapshot>(
            cancellationToken: ct);

        return snapshot ?? throw new InvalidOperationException();
    }
    public async Task<HttpResponseMessage> GetWorkContentAsync(Guid workId, CancellationToken ct = default)
    {
        var response = await client.GetAsync(
            $"/works/{workId}/content",
            HttpCompletionOption.ResponseHeadersRead,
            ct);

        response.EnsureSuccessStatusCode();
        return response;
    }
}