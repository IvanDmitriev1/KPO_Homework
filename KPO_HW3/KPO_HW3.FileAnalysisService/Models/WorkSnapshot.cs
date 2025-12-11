namespace KPO_HW3.FileAnalysisService.Models;

public sealed class WorkSnapshot
{
    public Guid Id { get; init; }
    public required Guid StudentId { get; init; }
    public required Guid AssignmentId { get; init; }
    public required string OriginalFileName { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}