using System.Text.Json.Serialization;

namespace KPO_HW3.Api.Models;

public sealed class WorkSnapshot
{
    [JsonPropertyName("id")]
    public required Guid WorkId { get; init; }

    [JsonPropertyName("studentId")]
    public required Guid StudentId { get; init; }

    [JsonPropertyName("assignmentId")]
    public required Guid AssignmentId { get; init; }
}