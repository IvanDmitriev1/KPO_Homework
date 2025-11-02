namespace KPO_HW2.Data.Abstractions;

internal interface IDbContextFactory<out TDbContext>
    where TDbContext : DbContext
{
    public string ConnectionString { get; }

    TDbContext Create();
    
    IAsyncDbConnection CreateAsyncDbConnection();
}