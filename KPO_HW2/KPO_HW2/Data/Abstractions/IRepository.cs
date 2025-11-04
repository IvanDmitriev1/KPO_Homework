namespace KPO_HW2.Data.Abstractions;

internal interface IRepository<TEntity, TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(TId id, CancellationToken ct = default);
    Task<bool> ExistsAsync(TId id, CancellationToken ct = default);
}