using System.Text.Json.Serialization;

namespace KPO_HW3.FileStorageService.Domain.Entities;

public class Work
{
    public Guid Id { get; init; }
    public required Guid StudentId { get; init; }
    public required Guid AssignmentId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public required string FileId { get; init; }
}
