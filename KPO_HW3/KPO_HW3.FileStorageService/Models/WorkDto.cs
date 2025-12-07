using KPO_HW3.FileStorageService.Infrastructure.Data.Entities;

namespace KPO_HW3.FileStorageService.Models;

public sealed class WorkDto
{
    public Guid Id { get; init; }
    public required Guid StudentId { get; init; }
    public required Guid AssignmentId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public static class WorkDtoExtensions
{
    public static WorkDto ToDto(this Work work)
    {
        return new WorkDto()
        {
            Id = work.Id,
            StudentId = work.StudentId,
            AssignmentId = work.AssignmentId,
            CreatedAt = work.CreatedAt,
        };
    }
}