using FluentValidation;

namespace KPO_HW3.FileStorageService.Domain.Entities;

public class Work
{
    public required Guid Id { get; init; }
    public required Guid StudentId { get; init; }
    public required Guid AssignmentId { get; init; }
    public required string FilePath { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public class WorkValidator : AbstractValidator<Work>
{
    public WorkValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Work Id must be non-empty.");

        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("StudentId is required.");

        RuleFor(x => x.AssignmentId)
            .NotEmpty()
            .WithMessage("AssignmentId is required.");

        RuleFor(x => x.FilePath)
            .NotEmpty()
            .WithMessage("FilePath is required.")
            .MaximumLength(1024)
            .WithMessage("FilePath is too long.")
            .Must(path => Path.IsPathRooted(path) == false)
            .WithMessage("FilePath must be relative.");

        RuleFor(x => x.CreatedAt)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddMinutes(1))
            .WithMessage("CreatedAt cannot be in the future.")
            .GreaterThan(DateTimeOffset.UtcNow.AddYears(-5))
            .WithMessage("CreatedAt is too old.");
    }
}
