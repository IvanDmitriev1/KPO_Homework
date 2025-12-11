using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KPO_HW3.FileAnalysisService.Infrastructure.Data.Entities;

public sealed class PlagiarismReport
{
    public required Guid WorkId { get; init; }
    public required Guid StudentId { get; init; }
    public required string ContentHash { get; init; }
    public required double SimilarityScore { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public required List<PlagiarismReportMatch> Matches { get; init; }
}

class PlagiarismReportConfiguration : IEntityTypeConfiguration<PlagiarismReport>
{
    public void Configure(EntityTypeBuilder<PlagiarismReport> builder)
    {
        builder.HasKey(r => r.WorkId);

        builder.Property(r => r.StudentId)
            .IsRequired();

        builder.Property(r => r.SimilarityScore)
            .IsRequired();

        builder.Property(r => r.ContentHash)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.OwnsMany(r => r.Matches, mb =>
        {
            mb.WithOwner()
                .HasForeignKey("ReportWorkId");

            mb.Property<int>("Id");
            mb.HasKey("Id");

            mb.Property(m => m.MatchedWorkId)
                .IsRequired();

            mb.Property(m => m.SimilarityScore)
                .IsRequired();
        });
    }
}