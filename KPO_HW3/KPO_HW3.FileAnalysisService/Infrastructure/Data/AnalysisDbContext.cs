using KPO_HW3.FileAnalysisService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW3.FileAnalysisService.Infrastructure.Data;

public sealed class AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : DbContext(options)
{
    public DbSet<PlagiarismReport> PlagiarismReports => Set<PlagiarismReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PlagiarismReport>(builder =>
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.WorkId)
                .IsRequired();

            builder.Property(r => r.StudentId)
                .IsRequired();

            builder.Property(r => r.AssignmentId)
                .IsRequired();

            builder.Property(r => r.IsPlagiarized)
                .IsRequired();

            builder.Property(r => r.SimilarityScore)
                .IsRequired();

            builder.Property(r => r.ContentHash)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(r => r.Details)
                .HasMaxLength(2000);

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("now()");

            builder.HasIndex(r => new { r.AssignmentId, r.ContentHash });
            builder.HasIndex(r => r.WorkId);
        });
    }
}