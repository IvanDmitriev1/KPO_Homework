using KPO_HW2.Data.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace KPO_HW2.Data.Repositories.Cache;

internal class CachedCategoryRepository : ICategoryRepository
{
    public CachedCategoryRepository(
        ICurrentTransactionProvider ctx,
        IMemoryCache cache,
        TimeSpan? ttl = null)
    {
        _inner = new CategoryRepository(ctx);
        _cache = cache;
        _opts = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl ?? TimeSpan.FromMinutes(5) };
    }

    private readonly ICategoryRepository _inner;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _opts;


    private static string KAll => "Category:all";
    private static string KId(CategoryId id) => $"Category:id:{id}";
    private static string KExists(CategoryId id) => $"Category:exists:{id}";
    private static string KType(CategoryType t) => $"Category:type:{t}";

    public Task<Category?> GetByIdAsync(CategoryId id, CancellationToken ct = default)
        => _cache.GetOrCreateAsync(KId(id), async e =>
        {
            e.SetOptions(_opts);
            return await _inner.GetByIdAsync(id, ct);
        });

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default)
        => await _cache.GetOrCreateAsync(KAll, async e =>
        {
            e.SetOptions(_opts);
            return await _inner.GetAllAsync(ct);
        }) ?? [];

    public async Task AddAsync(Category entity, CancellationToken ct = default)
    {
        await _inner.AddAsync(entity, ct);
        Invalidate(entity);
    }

    public async Task UpdateAsync(Category entity, CancellationToken ct = default)
    {
        await _inner.UpdateAsync(entity, ct);
        Invalidate(entity);
    }

    public async Task<bool> DeleteAsync(CategoryId id, CancellationToken ct = default)
    {
        var ok = await _inner.DeleteAsync(id, ct);
        _cache.Remove(KAll); _cache.Remove(KId(id)); _cache.Remove(KExists(id));
        _cache.Remove(KType(CategoryType.Income));
        _cache.Remove(KType(CategoryType.Expense));
        return ok;
    }

    public Task<bool> ExistsAsync(CategoryId id, CancellationToken ct = default)
        => _cache.GetOrCreateAsync(KExists(id), async e => { e.SetOptions(_opts); return await _inner.ExistsAsync(id, ct); });

    public async Task<IReadOnlyList<Category>> GetByType(CategoryType type)
        => await _cache.GetOrCreateAsync(KType(type), async e =>
        {
            e.SetOptions(_opts);
            return await _inner.GetByType(type);
        }) ?? [];

    private void Invalidate(Category entity)
    {
        _cache.Remove(KAll);
        _cache.Remove(KId(entity.Id));
        _cache.Remove(KExists(entity.Id));
        _cache.Remove(KType(entity.CategoryType));
    }
}