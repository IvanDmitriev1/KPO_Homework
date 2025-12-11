using Microsoft.EntityFrameworkCore;

namespace KPO_HW3.FileAnalysisService.Infrastructure.Data;

public sealed class AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : DbContext(options)
{
    public DbSet<PlagiarismReport> PlagiarismReports => Set<PlagiarismReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PlagiarismReportConfiguration());
    }
}