namespace KPO_HW3.Api.Infrastructure.Exceptions
{
    public class PlagiarismReportNotFoundException(Guid workId)
        : Exception($"Plagiarism report '{workId}' was not found.")
    {
        public Guid WorkId { get; } = workId;
    }
}