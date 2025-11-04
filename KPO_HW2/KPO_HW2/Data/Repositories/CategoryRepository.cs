using Dapper;
using KPO_HW2.Data.Abstractions;
using KPO_HW2.Exceptions;
using KPO_HW2.Models;
using Microsoft.Data.Sqlite;

namespace KPO_HW2.Data.Repositories;

internal class CategoryRepository(ICurrentTransactionProvider provider) : BaseRepository(provider), ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(CategoryId id, CancellationToken ct = default)
    {
        const string sql = "SELECT Id, CategoryType, Name FROM Category WHERE Id = @Id;";
        return await Connection.QuerySingleOrDefaultAsync<Category>(
            new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: ct));
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT Id, CategoryType, Name FROM Category ORDER BY CategoryType, Name;";
        var list = await Connection.QueryAsync<Category>(
            new CommandDefinition(sql, transaction: Transaction, cancellationToken: ct));
        return list.ToList();
    }

    public async Task AddAsync(Category entity, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO Category (Id, CategoryType, Name)
            VALUES (@Id, @CategoryType, @Name);
        """;

        try
        {
            await Connection.ExecuteAsync(new CommandDefinition(sql, new
            {
                entity.Id,
                CategoryType = entity.CategoryType.ToString(),
                entity.Name
            }, Transaction, cancellationToken: ct));
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            throw new DuplicateException(
                entityName: "Category",
                fieldName: "Name",
                value: entity.Name,
                innerException: ex);

        }
    }

    public async Task UpdateAsync(Category entity, CancellationToken ct = default)
    {
        const string sql = "UPDATE Category SET CategoryType = @CategoryType, Name = @Name WHERE Id = @Id;";

        try
        {
            await Connection.ExecuteAsync(new CommandDefinition(sql, new
            {
                entity.Id,
                CategoryType = entity.CategoryType.ToString(),
                entity.Name
            }, Transaction, cancellationToken: ct));
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            throw new DuplicateException(
                entityName: "Category",
                fieldName: "Name",
                value: entity.Name,
                innerException: ex);
        }
    }

    public async Task<bool> DeleteAsync(CategoryId id, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM Category WHERE Id = @Id;";
        var rows = await Connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: ct));
        return rows == 1;
    }

    public async Task<bool> ExistsAsync(CategoryId id, CancellationToken ct = default)
    {
        const string sql = "SELECT 1 FROM Category WHERE Id = @Id LIMIT 1;";
        var val = await Connection.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: ct));
        return val.HasValue;
    }

    public async Task<IReadOnlyList<Category>> GetByType(CategoryType type)
    {
        const string sql = "SELECT Id, CategoryType, Name FROM Category WHERE CategoryType = @type ORDER BY Name;";
        var list = await Connection.QueryAsync<Category>(sql, new { type }, Transaction);
        return list.ToList();
    }
}