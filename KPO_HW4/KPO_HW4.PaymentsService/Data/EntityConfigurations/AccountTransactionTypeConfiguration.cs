using KPO_HW4.PaymentsService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KPO_HW4.PaymentsService.Data.EntityConfigurations;

internal class AccountTransactionTypeConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.HasKey(at => at.Id);

        builder.Property(at => at.Id)
            .HasConversion(
                id => id.Value,
                value => new PaymentTransactionId(value))
            .ValueGeneratedOnAdd();

        builder.Property(at => at.AccountId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new AccountId(value));

        builder.Property(at => at.ReferenceId)
            .IsRequired();

        builder.Property(at => at.Type)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(at => at.AmountMinor)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(x => x.FailureCode)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(at => at.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAddOrUpdate();



        builder.HasIndex(x => x.ReferenceId)
            .IsUnique();
    }
}