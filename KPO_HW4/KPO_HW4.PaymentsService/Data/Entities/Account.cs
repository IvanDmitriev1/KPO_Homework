using KPO_HW4.Shared.Contracts;

namespace KPO_HW4.PaymentsService.Data.Entities;

public sealed class Account
{
    public AccountId Id { get; init; }
    public required UserId UserId { get; init; }

    public Int64 BalanceMinor { get; private set; }

    public decimal Balance => BalanceMinor / 100m;
}