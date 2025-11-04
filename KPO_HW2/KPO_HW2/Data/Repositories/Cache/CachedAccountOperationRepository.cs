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
        _opts = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl ?? TimeSpan.FromMinutes(1)
        };
    }

    private readonly IAccountOperationRepository _inner;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _opts;

    private static string KAll => "AccountOperation:all";
    private static string KId(AccountOperationId id) => $"AccountOperation:id:{id}";
    private static string KExists(AccountOperationId id) => $"AccountOperation:exists:{id}";
    private static string KByAccount(BankAccountId accountId) => $"AccountOperation:byAccount:{accountId}";
    private static string KHasCategory(CategoryId categoryId) => $"AccountOperation:hasCategory:{categoryId}";

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
        Invalidate(entity);
    }

    public async Task UpdateAsync(AccountOperation entity, CancellationToken ct = default)
    {
        await _inner.UpdateAsync(entity, ct);
        Invalidate(entity);
    }

    public async Task<bool> DeleteAsync(AccountOperationId id, CancellationToken ct = default)
    {
        var existing = await _inner.GetByIdAsync(id, ct);

        var ok = await _inner.DeleteAsync(id, ct);
        if (!ok)
            return false;

        if (existing is not null)
        {
            Invalidate(existing);
        }
        else
        {
            Invalidate(id);
        }

        return true;
    }

    public async Task<bool> ExistsAsync(AccountOperationId id, CancellationToken ct = default)
        => await _cache.GetOrCreateAsync(KExists(id), async e =>
        {
            e.SetOptions(_opts);
            return await _inner.ExistsAsync(id, ct);
        });

    public async Task<IReadOnlyList<AccountOperation>> GetByAccount(
        BankAccountId accountId,
        CancellationToken ct = default)
        => await _cache.GetOrCreateAsync(KByAccount(accountId), async e =>
        {
            e.SetOptions(_opts);
            return await _inner.GetByAccount(accountId, ct);
        }) ?? [];

    public async Task<bool> HasOperationsWithCategory(
        CategoryId categoryId,
        CancellationToken ct = default)
        => await _cache.GetOrCreateAsync(KHasCategory(categoryId), async e =>
        {
            e.SetOptions(_opts);
            return await _inner.HasOperationsWithCategory(categoryId, ct);
        });

    private void Invalidate(AccountOperationId id)
    {
        _cache.Remove(KAll);
        _cache.Remove(KId(id));
        _cache.Remove(KExists(id));
    }

    private void Invalidate(AccountOperation entity)
    {
        Invalidate(entity.Id);
        _cache.Remove(KByAccount(entity.BankAccountId));
        _cache.Remove(KHasCategory(entity.CategoryId));
    }
}
