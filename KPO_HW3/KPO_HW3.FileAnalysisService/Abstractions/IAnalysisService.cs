using DotNext;

namespace KPO_HW3.FileAnalysisService.Abstractions;

public interface IAnalysisService
{
    Task<Result<PlagiarismReport>> AnalyzeAsync(
        Guid workId,
        CancellationToken ct = default);

}