using System.Data;
using System.Data.Common;

namespace KPO_HW2.Data.Abstractions;

internal abstract class DbContext : IAsyncDisposable, ICurrentTransactionProvider
{
    protected DbContext(IAsyncDbConnection connection)
    {
        _connection = connection;
        CurrentTransaction = connection.BeginTransaction(Isolation);
    }

    private const IsolationLevel Isolation = IsolationLevel.ReadCommitted;
    private readonly IAsyncDbConnection _connection;
    private bool _disposed;

    public DbTransaction CurrentTransaction { get; private set; }

    public virtual async Task CommitAsync(CancellationToken ct = default)
    {
        EnsureNotCompleted();

        try
        {
            await CurrentTransaction.CommitAsync(ct);
        }
        finally
        {
            await CurrentTransaction.DisposeAsync();
            CurrentTransaction = await _connection.BeginTransactionAsync(Isolation, ct);
        }
    }
    public virtual async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_disposed)
            return;

        try
        {
            await CurrentTransaction.RollbackAsync(ct);
        }
        finally
        {
            await CurrentTransaction.DisposeAsync();
            CurrentTransaction = await _connection.BeginTransactionAsync(Isolation, ct);
        }
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_disposed)
            return;

        _disposed = true;

        try
        {
            if (CurrentTransaction.Connection is not null)
            {
                await CurrentTransaction.RollbackAsync();
            }
        }
        finally
        {
            await CurrentTransaction.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }

    private void EnsureNotCompleted()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(DbContext));
    }


}