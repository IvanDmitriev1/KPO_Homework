namespace KPO_HW2.Models;

public readonly record struct Money
{
    public Int64 AmountMinor { get; init; }
    public CurrencyCode CurrencyCode { get; init; }

    public decimal Major => AmountMinor / 100m;

    public override string ToString()
    {
        return $"{Major:N2} {CurrencyCode}";
    }

    public static Money Zero(CurrencyCode currency) => new()
    {
        AmountMinor = 0,
        CurrencyCode = currency
    };

    public static Money Create(decimal amount, CurrencyCode code) => new()
    {
        AmountMinor = (Int64)decimal.Round(amount * 100m, 0, MidpointRounding.AwayFromZero),
        CurrencyCode = code
    };

    public static Money operator +(Money left, Money right)
    {
        if (left.CurrencyCode != right.CurrencyCode)
        {
            throw new InvalidOperationException("Cannot add Money values with different currency codes.");
        }

        return new Money
        {
            AmountMinor = left.AmountMinor + right.AmountMinor,
            CurrencyCode = left.CurrencyCode
        };
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.CurrencyCode != right.CurrencyCode)
        {
            throw new InvalidOperationException("Cannot add Money values with different currency codes.");
        }

        return new Money
        {
            AmountMinor = left.AmountMinor - right.AmountMinor,
            CurrencyCode = left.CurrencyCode
        };
    }
}