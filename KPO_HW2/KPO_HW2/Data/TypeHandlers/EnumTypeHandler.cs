using System.Data;
using Dapper;

namespace KPO_HW2.Data.TypeHandlers;

internal class EnumTypeHandler<TEnum> : SqlMapper.TypeHandler<TEnum> 
    where TEnum : struct, Enum
{
    public override void SetValue(IDbDataParameter parameter, TEnum value)
    {
        parameter.Value = value.ToString();
        parameter.DbType = DbType.String;
    }

    public override TEnum Parse(object value)
    {
        string str = (string)value;
        return Enum.Parse<TEnum>(str);
    }
}