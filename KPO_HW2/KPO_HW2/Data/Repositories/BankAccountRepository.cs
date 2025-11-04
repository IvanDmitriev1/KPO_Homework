using Dapper;
using KPO_HW2.Data.Abstractions;
using KPO_HW2.Data.Extensions;
using KPO_HW2.Exceptions;
using Microsoft.Data.Sqlite;
using System.Data;

namespace KPO_HW2.Data.Repositories;

internal sealed class BankAccountRepository(ICurrentTransactionProvider provider) : BaseRepository(provider), IBankAccountRepository
{
    public async Task AddAsync(BankAccount entity, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO BankAccount (Id, Name, Balance_AmountMinor, Balance_CurrencyCode)
                           VALUES (@Id, @Name, @Balance_AmountMinor, @Balance_CurrencyCode);
                           """;

        var p = new DynamicParameters();
        p.Add("Id", entity.Id);
        p.Add("Name", entity.Name);
        p.AddMoney("Balance", entity.Balance);

        try
        {
            await Connection.ExecuteAsync(new CommandDefinition(sql, p, Transaction, cancellationToken: ct));
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            throw new DuplicateAccountNameException(entity.Name, ex);
        }
    }

    public async Task<bool> DeleteAsync(BankAccountId id, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM BankAccount WHERE Id = @Id;";
        var rows = await Connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: ct));
        return rows == 1;
    }

    public async Task<bool> ExistsAsync(BankAccountId id, CancellationToken ct = default)
    {
        const string sql = "SELECT 1 FROM BankAccount WHERE Id = @Id LIMIT 1;";
        var exists = await Connection.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: ct));
        return exists.HasValue;
    }

    public async Task<IReadOnlyList<BankAccount>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = """
                           SELECT
                               Id,
                               Name,
                               Balance_AmountMinor AS AmountMinor,
                               Balance_CurrencyCode AS CurrencyCode
                           FROM BankAccount
                           ORDER BY Name;
                           """;

        var list = await Connection.QueryAsync<BankAccount, Money, BankAccount>(
            new CommandDefinition(sql, transaction: Transaction, cancellationToken: ct),
            (acc, money) => new BankAccount { Id = acc.Id, Name = acc.Name, Balance = money },
            splitOn: "AmountMinor");

        return list.ToList();
    }

    public async Task<BankAccount?> GetByIdAsync(BankAccountId id, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT
                               Id,
                               Name,
                               Balance_AmountMinor AS AmountMinor,
                               Balance_CurrencyCode AS CurrencyCode
                           FROM BankAccount
                           WHERE Id = @Id;
                           """;

        var rows = await Connection.QueryAsync<BankAccount, Money, BankAccount>(
            new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: ct),
            (acc, money) => new BankAccount { Id = acc.Id, Name = acc.Name, Balance = money },
            splitOn: "AmountMinor");

        return rows.SingleOrDefault();
    }

    public async Task<BankAccount?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT
                               Id,
                               Name,
                               Balance_AmountMinor AS AmountMinor,
                               Balance_CurrencyCode AS CurrencyCode
                           FROM BankAccount
                           WHERE Name = @Name;
                           """;

        var rows = await Connection.QueryAsync<BankAccount, Money, BankAccount>(
            new CommandDefinition(sql, new { Name = name }, Transaction, cancellationToken: ct),
            (acc, money) => new BankAccount { Id = acc.Id, Name = acc.Name, Balance = money },
            splitOn: "AmountMinor");

        return rows.SingleOrDefault();
    }

    public async Task UpdateAsync(BankAccount entity, CancellationToken ct = default)
    {
        const string sql = "UPDATE BankAccount SET Name = @Name WHERE Id = @Id;";
        var rows = await Connection.ExecuteAsync(
            new CommandDefinition(sql, new { entity.Id, entity.Name }, Transaction, cancellationToken: ct));

        if (rows != 1)
            throw new DBConcurrencyException("BankAccount update failed.");
    }
}
