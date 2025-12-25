using KPO_HW4.PaymentsService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KPO_HW4.PaymentsService.Data.EntityConfigurations;

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

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_accounts_balance_non_negative", $"""
                                                                      "{nameof(Account.BalanceMinor)}" >= 0
                                                                      """);
        });
    }
}