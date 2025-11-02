using KPO_HW2.Data.Abstractions;
using KPO_HW2.Data.Repositories.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace KPO_HW2.Data.Services;

internal class AppDbContext : DbContext
{
    public AppDbContext(IAsyncDbConnection connection, IMemoryCache cache) : base(connection)
    {
        BankAccountRepository = new CachedBankAccountRepository(this, cache);
        CategoryRepository = new CachedCategoryRepository(this, cache);
        AccountOperationRepository = new CachedAccountOperationRepository(this, cache);
    }

    public IBankAccountRepository BankAccountRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IAccountOperationRepository AccountOperationRepository { get; }
}