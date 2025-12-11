using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KPO_HW3.FileStorageService.Infrastructure.Data.Entities;

public class Work
{
    public Guid Id { get; init; }
    public required Guid StudentId { get; init; }
    public required Guid AssignmentId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public required string OriginalFileName { get; init; }
    public Guid FileId { get; init; }
}

class WorkTypeConfiguration : IEntityTypeConfiguration<Work>
{
    public void Configure(EntityTypeBuilder<Work> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .ValueGeneratedOnAdd();

        builder.Property(w => w.FileId)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(w => w.OriginalFileName)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(w => w.AssignmentId)
            .IsRequired();

        builder.Property(w => w.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.HasIndex(w => new { w.StudentId, w.AssignmentId })
            .IsUnique();
    }
}