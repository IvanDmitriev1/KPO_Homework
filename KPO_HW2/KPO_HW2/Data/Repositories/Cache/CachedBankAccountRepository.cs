using KPO_HW2.Data.Abstractions;
using KPO_HW2.Data.Models;
using KPO_HW2.Data.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KPO_HW2.Data.Repositories.Cache;

internal class CachedBankAccountRepository : IBankAccountRepository
{
    private readonly IBankAccountRepository _inner;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _opts;

    public CachedBankAccountRepository(
        ICurrentTransactionProvider ctx,
        IMemoryCache cache,
        TimeSpan? ttl = null)
    {
        _inner = new BankAccountRepository(ctx); // внутренняя реальная реализация
        _cache = cache;
        _opts = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl ?? TimeSpan.FromMinutes(5) };
    }

    private static string KAll => "BankAccount:all";
    private static string KId(BankAccountId id) => $"BankAccount:id:{id}";
    private static string KName(string name) => $"BankAccount:name:{name}";
    private static string KExists(BankAccountId id) => $"BankAccount:exists:{id}";

    public Task<BankAccount?> GetByIdAsync(BankAccountId id, CancellationToken ct = default)
        => _cache.GetOrCreateAsync(KId(id), async _ =>
        {
            _.SetOptions(_opts);
            return await _inner.GetByIdAsync(id, ct);
        });

    public Task<IReadOnlyList<BankAccount>> GetAllAsync(CancellationToken ct = default)
        => _cache.GetOrCreateAsync(KAll, async _ =>
        {
            _.SetOptions(_opts);
            return await _inner.GetAllAsync(ct);
        });

    public async Task AddAsync(BankAccount entity, CancellationToken ct = default)
    {
        await _inner.AddAsync(entity, ct);
        Invalidate(entity);
    }

    public async Task UpdateAsync(BankAccount entity, CancellationToken ct = default)
    {
        await _inner.UpdateAsync(entity, ct);
        Invalidate(entity);
    }

    public async Task<bool> DeleteAsync(BankAccountId id, CancellationToken ct = default)
    {
        var ok = await _inner.DeleteAsync(id, ct);
        _cache.Remove(KAll);
        _cache.Remove(KId(id));
        _cache.Remove(KExists(id));
        return ok;
    }

    public Task<bool> ExistsAsync(BankAccountId id, CancellationToken ct = default)
        => _cache.GetOrCreateAsync(KExists(id), async _ =>
        {
            _.SetOptions(_opts);
            return await _inner.ExistsAsync(id, ct);
        });

    public Task<BankAccount?> GetByNameAsync(string name, CancellationToken ct = default)
        => _cache.GetOrCreateAsync(KName(name), async _ =>
        {
            _.SetOptions(_opts);
            return await _inner.GetByNameAsync(name, ct);
        });

    private void Invalidate(BankAccount entity)
    {
        _cache.Remove(KAll);
        _cache.Remove(KId(entity.Id));
        _cache.Remove(KExists(entity.Id));
        _cache.Remove(KName(entity.Name));
    }
}