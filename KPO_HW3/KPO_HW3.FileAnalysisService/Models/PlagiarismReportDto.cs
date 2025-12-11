namespace KPO_HW3.FileAnalysisService.Models;

public sealed class PlagiarismReportDto
{
    public class Match
    {
        public required Guid WorkId { get; init; }
        public int SimilarityScore { get; init; }
    }

    public required Guid WorkId { get; init; }

    public required bool IsPlagiarized { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }

    public required Match[] Matches { get; init; }
}

public static class PlagiarismReportDtoDtoExtensions
{
    public static PlagiarismReportDto ToDto(this PlagiarismReport work) => new()
    {
        WorkId = work.WorkId,
        IsPlagiarized = work.SimilarityScore > 0,
        CreatedAt = work.CreatedAt,
        Matches = work.Matches.Select(match => new PlagiarismReportDto.Match()
        {
            WorkId = match.MatchedWorkId,
            SimilarityScore = (int)(match.SimilarityScore * 100)
        }).ToArray()
    };
}