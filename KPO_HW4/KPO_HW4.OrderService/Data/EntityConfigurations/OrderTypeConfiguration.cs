using KPO_HW4.OrderService.Data.Entities;
using KPO_HW4.Shared.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KPO_HW4.OrderService.Data.EntityConfigurations;

internal sealed class OrderTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, v => new OrderId(v))
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasConversion(id => id.Value, v => new UserId(v));

        builder.Property(x => x.AmountMinor)
            .IsRequired();

        builder.Property(x => x.Description);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAddOrUpdate();

        builder.HasIndex(x => x.UserId);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_orders_amount_positive", $"""
                                                               "{nameof(Order.AmountMinor)}" > 0
                                                               """);
        });
    }
}