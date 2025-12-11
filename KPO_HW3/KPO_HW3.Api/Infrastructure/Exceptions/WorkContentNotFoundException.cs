namespace KPO_HW3.Api.Infrastructure.Exceptions;

public sealed class WorkContentNotFoundException(Guid workId) : Exception($"Content for work '{workId}' was not found.")
{
    public Guid WorkId { get; } = workId;
}