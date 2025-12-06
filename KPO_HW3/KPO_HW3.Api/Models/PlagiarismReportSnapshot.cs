using System.Text.Json.Serialization;

namespace KPO_HW3.Api.Models;

public sealed class PlagiarismReportSnapshot
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("workId")]
    public required Guid WorkId { get; init; }

    [JsonPropertyName("studentId")]
    public required Guid StudentId { get; init; }

    [JsonPropertyName("assignmentId")]
    public required Guid AssignmentId { get; init; }

    [JsonPropertyName("isPlagiarized")]
    public required bool IsPlagiarized { get; init; }

    [JsonPropertyName("similarityScore")]
    public required double SimilarityScore { get; init; }

    [JsonPropertyName("contentHash")]
    public required string ContentHash { get; init; }

    [JsonPropertyName("details")]
    public required string? Details { get; init; }

    [JsonPropertyName("createdAt")]
    public required DateTimeOffset CreatedAt { get; init; }
}