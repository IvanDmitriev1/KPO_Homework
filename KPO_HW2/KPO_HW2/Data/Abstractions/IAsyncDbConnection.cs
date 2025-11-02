using System.Data;
using System.Data.Common;

namespace KPO_HW2.Data.Abstractions;

public interface IAsyncDbConnection : IAsyncDisposable
{
    public DbTransaction BeginTransaction(IsolationLevel isolationLevel);
    public ValueTask<DbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken ct = default);
}