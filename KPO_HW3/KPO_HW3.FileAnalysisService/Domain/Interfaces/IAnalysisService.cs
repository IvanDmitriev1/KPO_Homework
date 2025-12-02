using DotNext;
using KPO_HW3.FileAnalysisService.Domain.Entities;

namespace KPO_HW3.FileAnalysisService.Domain.Interfaces;

public interface IAnalysisService
{
    Task<Result<PlagiarismReport>> AnalyzeAsync(
        Guid workId,
        CancellationToken ct = default);

}