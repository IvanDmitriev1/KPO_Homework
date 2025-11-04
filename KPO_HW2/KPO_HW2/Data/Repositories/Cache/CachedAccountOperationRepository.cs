using KPO_HW2.Data.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace KPO_HW2.Data.Repositories.Cache;

internal sealed class CachedAccountOperationRepository : IAccountOperationRepository
{
    public CachedAccountOperationRepository(
        ICurrentTransactionProvider ctx,
        IMemoryCache cache,
        TimeSpan? ttl = null)
    {
        _inner = new AccountOperationRepository(ctx);
        _cache = cache;
        _opts = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl ?? TimeSpan.FromMinutes(1) };
    }

    private readonly IAccountOperationRepository _inner;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _opts;

    private static string KAll => "AccountOperation:all";
    private static string KId(AccountOperationId id) => $"AccountOperation:id:{id}";
    private static string KExists(AccountOperationId id) => $"AccountOperation:exists:{id}";

    public Task<AccountOperation?> GetByIdAsync(AccountOperationId id, CancellationToken ct = default)
        => _cache.GetOrCreateAsync(KId(id), async e =>
        {
            e.SetOptions(_opts);
            return await _inner.GetByIdAsync(id, ct);
        });

    public async Task<IReadOnlyList<AccountOperation>> GetAllAsync(CancellationToken ct = default)
        => await _cache.GetOrCreateAsync(KAll, async e =>
        {
            e.SetOptions(_opts);
            return await _inner.GetAllAsync(ct);
        }) ?? [];

    public async Task AddAsync(AccountOperation entity, CancellationToken ct = default)
    {
        await _inner.AddAsync(entity, ct);
        Invalidate(entity.Id);
    }

    public async Task UpdateAsync(AccountOperation entity, CancellationToken ct = default)
    {
        await _inner.UpdateAsync(entity, ct);
        Invalidate(entity.Id);
    }

    public async Task<bool> DeleteAsync(AccountOperationId id, CancellationToken ct = default)
    {
        var ok = await _inner.DeleteAsync(id, ct);
        Invalidate(id);
        return ok;
    }

    public async Task<bool> ExistsAsync(AccountOperationId id, CancellationToken ct = default)
        => await _cache.GetOrCreateAsync(KExists(id), async e =>
        {
            e.SetOptions(_opts);
            return await _inner.ExistsAsync(id, ct);
        });

    private void Invalidate(AccountOperationId id)
    {
        _cache.Remove(KAll);
        _cache.Remove(KId(id));
        _cache.Remove(KExists(id));
    }
}