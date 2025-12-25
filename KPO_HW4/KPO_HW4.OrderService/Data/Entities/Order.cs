namespace KPO_HW4.OrderService.Data.Entities;

public sealed class Order
{
    public OrderId Id { get; init; }
    public required UserId UserId { get; init; }

    public required long AmountMinor { get; init; }
    public required string Description { get; init; }

    public OrderStatus Status { get; private set; } = OrderStatus.New;

    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Order Create(UserId userId, long amountMinor, string description) => new()
    {
        UserId = userId,
        AmountMinor = amountMinor,
        Description = description,
    };

    public void MarkFinished()
    {
        if (Status != OrderStatus.New)
            return;

        Status = OrderStatus.Finished;
    }

    public void MarkCancelled()
    {
        if (Status != OrderStatus.New)
            return;

        Status = OrderStatus.Cancelled;
    }
}