using System.Data;
using Dapper;
using KPO_HW2.Data.Models;

namespace KPO_HW2.Data.TypeHandlers;

internal class CurrencyCodeTypeHandler : SqlMapper.TypeHandler<CurrencyCode>
{
    public override void SetValue(IDbDataParameter parameter, CurrencyCode value)
    {
        parameter.Value = value.ToStringFast(true);
        parameter.DbType = DbType.String;
    }

    public override CurrencyCode Parse(object value)
    {
        string name = (string)value;
        return CurrencyCodeExtensions.Parse(name);
    }
}