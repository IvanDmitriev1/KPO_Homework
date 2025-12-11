using System.Net;
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

    public Task<Result<PlagiarismReportSnapshot>> GetReportsByWorkAsync(
        Guid workId,
        CancellationToken cancellationToken = default) =>
        client.GetAsync($"/works/{workId}/reports", cancellationToken)
            .ToJsonResultAsync<PlagiarismReportSnapshot>((code, body) =>
            {
                if (code == HttpStatusCode.NotFound)
                {
                    return new PlagiarismReportNotFoundException(workId);
                }

                return new DownstreamHttpException(code, body);
            }, ct: cancellationToken);
}
