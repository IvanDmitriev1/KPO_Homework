using KPO_HW4.PaymentsService.Data.Entities;
using KPO_HW4.PaymentsService.Data.EntityConfigurations;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW4.PaymentsService.Data;

public sealed class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<PaymentTransaction> Transactions => Set<PaymentTransaction>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AccountTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AccountTransactionTypeConfiguration());

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}