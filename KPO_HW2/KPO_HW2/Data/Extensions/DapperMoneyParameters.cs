using Dapper;
using System.Data;

namespace KPO_HW2.Data.Extensions;

internal static class DapperMoneyParameters
{
    public static void AddMoney(this DynamicParameters p, string prefix, Money money)
    {
        p.Add($"{prefix}_{nameof(Money.AmountMinor)}", money.AmountMinor, DbType.Int64);
        p.Add($"{prefix}_{nameof(Money.CurrencyCode)}", money.CurrencyCode.ToStringFast(true), DbType.String);
    }
}