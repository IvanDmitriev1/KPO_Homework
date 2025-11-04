using System.Data;
using Dapper;

namespace KPO_HW2.Data.TypeHandlers;

internal class DateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset>
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        parameter.Value = value;
    }

    public override DateTimeOffset Parse(object value)
        => DateTimeOffset.Parse((string)value);
}