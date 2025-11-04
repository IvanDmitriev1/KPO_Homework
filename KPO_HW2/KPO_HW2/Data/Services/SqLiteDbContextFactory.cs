using Dapper;
using KPO_HW2.Data.Abstractions;
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
    }

    private readonly IMemoryCache _memoryCache;

    public string ConnectionString { get; }

    public AppDbContext Create()
    {
        var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        return new AppDbContext(new SqLiteDbConnectionAdapter(connection), _memoryCache);
    }
}