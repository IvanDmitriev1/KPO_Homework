namespace KPO_HW3.FileAnalysisService.Infrastructure.Data.Entities;

public class PlagiarismReportMatch
{
    public required Guid MatchedWorkId { get; init; }
    public required double SimilarityScore { get; init; }
}