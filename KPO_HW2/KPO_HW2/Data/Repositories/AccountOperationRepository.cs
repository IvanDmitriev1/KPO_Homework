using Dapper;
using KPO_HW2.Data.Abstractions;
using KPO_HW2.Data.Extensions;

namespace KPO_HW2.Data.Repositories;

internal class AccountOperationRepository(ICurrentTransactionProvider provider) : BaseRepository(provider), IAccountOperationRepository
{
    public async Task<AccountOperation?> GetByIdAsync(AccountOperationId id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                Id,
                BankAccountId,
                DateOfOperation,
                Description,
                CategoryId,
                Amount_AmountMinor   AS AmountMinor,
                Amount_CurrencyCode  AS CurrencyCode
            FROM AccountOperation
            WHERE Id = @Id;
        """;

        var rows = await Connection.QueryAsync<AccountOperation, Money, AccountOperation>(
            new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: ct),
            (op, money) => new AccountOperation
            {
                Id = op.Id,
                BankAccountId = op.BankAccountId,
                Amount = money,
                DateOfOperation = op.DateOfOperation,
                Description = op.Description,
                CategoryId = op.CategoryId
            },
            splitOn: "AmountMinor");

        return rows.SingleOrDefault();
    }

    public async Task<IReadOnlyList<AccountOperation>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                Id,
                BankAccountId,
                DateOfOperation,
                Description,
                CategoryId,
                Amount_AmountMinor   AS AmountMinor,
                Amount_CurrencyCode  AS CurrencyCode
            FROM AccountOperation
            ORDER BY DateOfOperation DESC, Id;
        """;

        var list = await Connection.QueryAsync<AccountOperation, Money, AccountOperation>(
            new CommandDefinition(sql, transaction: Transaction, cancellationToken: ct),
            (op, money) => new AccountOperation
            {
                Id = op.Id,
                BankAccountId = op.BankAccountId,
                Amount = money,
                DateOfOperation = op.DateOfOperation,
                Description = op.Description,
                CategoryId = op.CategoryId
            },
            splitOn: "AmountMinor");

        return list.ToList();
    }

    public async Task AddAsync(AccountOperation entity, CancellationToken ct = default)
    {
        const string sql = """
                               INSERT INTO AccountOperation
                                   (Id, BankAccountId, Amount_AmountMinor, Amount_CurrencyCode, DateOfOperation, Description, CategoryId)
                               VALUES
                                   (@Id, @BankAccountId, @Amount_AmountMinor, @Amount_CurrencyCode, @DateOfOperation, @Description, @CategoryId);
                           """;

        var p = new DynamicParameters();
        p.Add("Id", entity.Id);
        p.Add("BankAccountId", entity.BankAccountId);
        p.AddMoney("Amount", entity.Amount);
        p.Add("DateOfOperation", entity.DateOfOperation);
        p.Add("Description", entity.Description);
        p.Add("CategoryId", entity.CategoryId);

        await Connection.ExecuteAsync(new CommandDefinition(sql, p, Transaction, cancellationToken: ct));
    }

    public async Task UpdateAsync(AccountOperation entity, CancellationToken ct = default)
    {
        const string sql = """
                               UPDATE AccountOperation
                               SET BankAccountId        = @BankAccountId,
                                   Amount_AmountMinor   = @Amount_AmountMinor,
                                   Amount_CurrencyCode  = @Amount_CurrencyCode,
                                   DateOfOperation      = @DateOfOperation,
                                   Description          = @Description,
                                   CategoryId           = @CategoryId
                               WHERE Id = @Id;
                           """;

        var p = new DynamicParameters();
        p.Add("Id", entity.Id);
        p.Add("BankAccountId", entity.BankAccountId);
        p.AddMoney("Amount", entity.Amount);
        p.Add("DateOfOperation", entity.DateOfOperation);
        p.Add("Description", entity.Description);
        p.Add("CategoryId", entity.CategoryId);

        await Connection.ExecuteAsync(new CommandDefinition(sql, p, Transaction, cancellationToken: ct));
    }

    public async Task<bool> DeleteAsync(AccountOperationId id, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM AccountOperation WHERE Id = @Id;";
        var rows = await Connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: ct));
        return rows == 1;
    }

    public async Task<bool> ExistsAsync(AccountOperationId id, CancellationToken ct = default)
    {
        const string sql = "SELECT 1 FROM AccountOperation WHERE Id = @Id LIMIT 1;";
        var val = await Connection.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: ct));
        return val.HasValue;
    }

    public async Task<IReadOnlyList<AccountOperation>> GetByAccount(BankAccountId accountId, CancellationToken ct)
    {
        const string sql = """
                               SELECT
                                   Id,
                                   BankAccountId,
                                   DateOfOperation,
                                   Description,
                                   CategoryId,
                                   Amount_AmountMinor  AS AmountMinor,
                                   Amount_CurrencyCode AS CurrencyCode
                               FROM AccountOperation
                               WHERE BankAccountId = @BankAccountId
                               ORDER BY DateOfOperation DESC, Id;
                           """;

        var list = await Connection.QueryAsync<AccountOperation, Money, AccountOperation>(
            new CommandDefinition(sql, parameters: new
            {
                BankAccountId = accountId,

            }, transaction: Transaction, cancellationToken: ct),
            (op, money) => new AccountOperation
            {
                Id = op.Id,
                BankAccountId = op.BankAccountId,
                Amount = money,
                DateOfOperation = op.DateOfOperation,
                Description = op.Description,
                CategoryId = op.CategoryId
            },
            splitOn: "AmountMinor");

        return list.ToList();
    }

    public async Task<bool> HasOperationsWithCategory(CategoryId categoryId, CancellationToken ct)
    {
        const string sql = "SELECT 1 FROM AccountOperation WHERE CategoryId = @CategoryId LIMIT 1;";
        var val = await Connection.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { CategoryId = categoryId },
            Transaction, cancellationToken: ct));
        return val.HasValue;
    }
}