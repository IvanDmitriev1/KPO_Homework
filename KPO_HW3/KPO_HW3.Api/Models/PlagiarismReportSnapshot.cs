namespace KPO_HW3.Api.Models;

public sealed class PlagiarismReportSnapshot
{
    public class Match
    {
        public required Guid WorkId { get; init; }
        public int SimilarityScore { get; init; }
    }

    public required Guid WorkId { get; init; }

    public required bool IsPlagiarized { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }

    public required List<Match> Matches { get; init; }
}