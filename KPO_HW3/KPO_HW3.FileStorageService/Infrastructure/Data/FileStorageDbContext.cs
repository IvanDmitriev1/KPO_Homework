using KPO_HW3.FileStorageService.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW3.FileStorageService.Infrastructure.Data;

public class FileStorageDbContext(DbContextOptions<FileStorageDbContext> options) : DbContext(options)
{
    public DbSet<Work> Works => Set<Work>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new WorkTypeConfiguration());
    }
}