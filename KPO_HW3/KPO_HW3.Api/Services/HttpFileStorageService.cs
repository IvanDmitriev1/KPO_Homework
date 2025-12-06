using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace KPO_HW3.Api.Services;

public sealed class HttpFileStorageService(HttpClient client) : IFileStorageService
{
    public async Task<WorkSnapshot?> GetWorkAsync(Guid workId, CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync($"/works/{workId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var snapshot = await JsonSerializer.DeserializeAsync<WorkSnapshot>(stream, cancellationToken: cancellationToken);

        return snapshot;
    }

    public async Task<WorkSnapshot> UploadWorkAsync(
        Guid studentId,
        Guid assignmentId,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(studentId.ToString()), "studentId");
        content.Add(new StringContent(assignmentId.ToString()), "assignmentId");

        var fileStream = file.OpenReadStream();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
        content.Add(fileContent, "file", file.FileName);

        var response = await client.PostAsync("/works/upload", content, cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var snapshot = await JsonSerializer.DeserializeAsync<WorkSnapshot>(stream, cancellationToken: cancellationToken);

        if (snapshot is null)
        {
            throw new InvalidOperationException("FileStorageService returned empty response.");
        }

        return snapshot;
    }

    public async Task<HttpResponseMessage?> GetWorkContentResponseAsync(Guid workId, CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync($"/works/{workId}/content", HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return response;
    }
}
