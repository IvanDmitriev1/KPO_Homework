namespace KPO_HW3.FileAnalysisService.Domain.Exceptions;

public sealed class WorkNotFoundException(Guid workId)
    : Exception($"Work with id '{workId}' was not found in FileStorageService.")
{
    public Guid WorkId { get; } = workId;
}