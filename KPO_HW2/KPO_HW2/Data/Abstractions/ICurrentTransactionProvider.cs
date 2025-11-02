using System.Data.Common;

namespace KPO_HW2.Data.Abstractions;

internal interface ICurrentTransactionProvider
{
    DbTransaction CurrentTransaction { get; }
}