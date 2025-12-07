namespace KPO_HW3.FileStorageService.Infrastructure.Data.Entities;

public class Work
{
    public Guid Id { get; init; }
    public required Guid StudentId { get; init; }
    public required Guid AssignmentId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public required string OriginalFileName { get; init; }
    public Guid FileId { get; init; }
}
