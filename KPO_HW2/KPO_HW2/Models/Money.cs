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

    public static Money Create(Int64 amount, CurrencyCode code) => new()
    {
        AmountMinor = amount,
        CurrencyCode = code
    };
}