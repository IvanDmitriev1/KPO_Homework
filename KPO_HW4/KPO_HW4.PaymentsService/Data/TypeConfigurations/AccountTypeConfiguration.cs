using KPO_HW4.PaymentsService.Data.Entities;
using KPO_HW4.Shared.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KPO_HW4.PaymentsService.Data.TypeConfigurations;

internal sealed class AccountTypeConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasConversion(
                id => id.Value,
                value => new AccountId(value))
            .ValueGeneratedOnAdd();

        builder.Property(a => a.UserId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Property(a => a.BalanceMinor)
            .IsRequired();

        builder.HasIndex(a => a.UserId)
            .IsUnique();
    }
}