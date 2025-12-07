using KPO_HW3.FileStorageService.Infrastructure.Data.Entities;
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
        });
    }
}