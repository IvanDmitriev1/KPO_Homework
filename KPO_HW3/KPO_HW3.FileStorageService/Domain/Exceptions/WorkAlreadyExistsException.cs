namespace KPO_HW3.FileStorageService.Domain.Exceptions;

public sealed class WorkAlreadyExistsException(Guid studentId, Guid assignmentId)
    : Exception($"Work for student '{studentId}' and assignment '{assignmentId}' already exists.")
{
    public Guid StudentId { get; } = studentId;
    public Guid AssignmentId { get; } = assignmentId;
}