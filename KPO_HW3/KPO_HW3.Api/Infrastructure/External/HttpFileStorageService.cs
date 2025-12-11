using DotNext;
using KPO_HW3.Api.Infrastructure.Exceptions;
using KPO_HW3.Api.Infrastructure.Extensions;
using System.Net;
using System.Net.Http.Headers;

namespace KPO_HW3.Api.Infrastructure.External;

public sealed class HttpFileStorageService(HttpClient client) : IFileStorageService
{
    public Task<Result<WorkSnapshot>> GetWorkAsync(Guid workId, CancellationToken ct = default) =>
        client.GetAsync($"/works/{workId}", ct)
            .ToJsonResultAsync<WorkSnapshot>(static (code, body) =>
            {
                if (code == HttpStatusCode.NotFound)
                    return new WorkNotFoundException(body);

                return new DownstreamHttpException(code, body);
            }, ct);

    public async Task<Result<WorkSnapshot>> UploadWorkAsync(
        Guid studentId,
        Guid assignmentId,
        IFormFile file,
        CancellationToken ct = default)
    {
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(studentId.ToString()), "studentId");
        content.Add(new StringContent(assignmentId.ToString()), "assignmentId");

        var fileStream = file.OpenReadStream();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
        content.Add(fileContent, "file", file.FileName);

        return await client.PostAsync("/works/upload", content, ct)
            .ToJsonResultAsync<WorkSnapshot>(static (code, body) =>
            {
                return code switch
                {
                    HttpStatusCode.NotFound => new WorkNotFoundException(body),
                    HttpStatusCode.Conflict => new WorkAlreadyExistsException(body),
                    _ => new DownstreamHttpException(code, body)
                };
            }, ct: ct);
    }

    public async Task<Result<HttpResponseMessage>> GetWorkContentResponseAsync(Guid workId, CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync($"/works/{workId}/content", HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Result.FromException<HttpResponseMessage>(new WorkContentNotFoundException(workId));

        response.EnsureSuccessStatusCode();
        return response;
    }
}
