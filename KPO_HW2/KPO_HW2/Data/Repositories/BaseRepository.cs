using System.Data;
using KPO_HW2.Data.Abstractions;

namespace KPO_HW2.Data.Repositories;

internal abstract class BaseRepository(ICurrentTransactionProvider transactionProvider)
{
    protected IDbTransaction Transaction => transactionProvider.CurrentTransaction;
    protected IDbConnection Connection => Transaction.Connection!;
}