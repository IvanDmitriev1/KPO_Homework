namespace KPO_HW3.FileStorageService.Infrastructure.Exceptions;

public sealed class WorkNotFoundException(Guid workId) : Exception($"Work '{workId}' was not found.")
{
    public Guid WorkId { get; } = workId;
}
