using Microsoft.EntityFrameworkCore;

namespace KPO_HW3.FileStorageService.Infrastructure.Data;

public class FileStorageDbContext(DbContextOptions<FileStorageDbContext> options) : DbContext(options)
{
    public DbSet<Work> Works => Set<Work>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Work>(builder =>
        {
            builder.ToTable("works");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.StudentId).IsRequired();
            builder.Property(w => w.AssignmentId).IsRequired();
            builder.Property(w => w.CreatedAt).IsRequired();
            builder.Property(w => w.FilePath)
                .HasMaxLength(512)
                .IsRequired();
        });
    }
}