using Dapper;
using KPO_HW2.Data.Abstractions;
using KPO_HW2.Data.Models;
using KPO_HW2.Data.TypeHandlers;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;

namespace KPO_HW2.Data.Services;

internal class SqLiteDbContextFactory : IDbContextFactory<AppDbContext>
{
    public SqLiteDbContextFactory(string connectionString, IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        ConnectionString = connectionString;

        SqlMapper.AddTypeHandler(new BankAccountId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new CategoryId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new AccountOperationId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new CurrencyCodeTypeHandler());
        SqlMapper.AddTypeHandler(new EnumTypeHandler<CategoryType>());
        SqlMapper.AddTypeHandler(new EnumTypeHandler<AccountOperationType>());
    }

    private readonly IMemoryCache _memoryCache;

    public string ConnectionString { get; }

    public AppDbContext Create()
    {
        var connection = new SqliteConnection(ConnectionString);
        return new AppDbContext(new SqLiteDbConnectionAdapter(connection), _memoryCache);
    }

    public IAsyncDbConnection CreateAsyncDbConnection()
    {
        return new SqLiteDbConnectionAdapter(new SqliteConnection(ConnectionString));
    }
}