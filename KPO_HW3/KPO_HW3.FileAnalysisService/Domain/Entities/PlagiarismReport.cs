using FluentValidation;

namespace KPO_HW3.FileAnalysisService.Domain.Entities;


public sealed class PlagiarismReport
{
    public Guid Id { get; init; }
    public required Guid WorkId { get; init; }

    public required Guid StudentId { get; init; }
    public required Guid AssignmentId { get; init; }

    /// <summary>
    /// true, если работа признана плагиатом.
    /// </summary>
    public required bool IsPlagiarized { get; init; }

    /// <summary>
    /// 0..1
    /// </summary>
    public required double SimilarityScore { get; init; }

    /// <summary>
    /// Хэш содержимого для поиска совпадений.
    /// </summary>
    public required string ContentHash { get; init; }

    /// <summary>
    /// Человеко-читаемое пояснение (например, "совпадение с работой X").
    /// </summary>
    public string? Details { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}