using DotNext;

namespace KPO_HW3.Api.Abstractions;

public interface IFileAnalysisService
{
    Task<Result<PlagiarismReportSnapshot>> AnalyzeWorkAsync(
        Guid workId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PlagiarismReportSnapshot>> GetReportsByWorkAsync(
        Guid workId,
        CancellationToken cancellationToken = default);
}