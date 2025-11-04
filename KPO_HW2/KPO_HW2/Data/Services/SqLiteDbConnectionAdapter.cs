using System.Data;
using System.Data.Common;
using KPO_HW2.Data.Abstractions;
using Microsoft.Data.Sqlite;

namespace KPO_HW2.Data.Services;

internal class SqLiteDbConnectionAdapter(SqliteConnection connection) : IAsyncDbConnection
{
    public ValueTask DisposeAsync() => connection.DisposeAsync();

    public DbTransaction BeginTransaction(IsolationLevel isolationLevel) =>
        connection.BeginTransaction(isolationLevel);

    public ValueTask<DbTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken ct = default) =>
        connection.BeginTransactionAsync(isolationLevel, ct);
}