using System.Data;
using Dapper;
using KPO_HW2.Data.Abstractions;

namespace KPO_HW2.Data.Services;

internal class SqLiteDbInitializer
{
    public SqLiteDbInitializer(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public async Task Initialize()
    {
        const string sql = """
                           PRAGMA foreign_keys = ON;
                           
                           CREATE TABLE BankAccount (
                               Id                    TEXT PRIMARY KEY CHECK (length(Id) = 36),
                               Name                  TEXT NOT NULL UNIQUE,
                               Balance_AmountMinor   INTEGER NOT NULL,
                               Balance_CurrencyCode  TEXT NOT NULL CHECK (Balance_CurrencyCode IN ('USD','RUB'))
                           );
                           
                           CREATE TABLE Category (
                               Id            TEXT PRIMARY KEY CHECK (length(Id) = 36),
                               CategoryType  TEXT NOT NULL CHECK (CategoryType IN ('Income','Expense')),
                               Name          TEXT NOT NULL,
                               UNIQUE (CategoryType, Name)
                           );
                           
                           CREATE TABLE AccountOperation (
                               Id                     TEXT PRIMARY KEY CHECK (length(Id) = 36),
                               AccountOperationType   TEXT NOT NULL CHECK (AccountOperationType IN ('Income','Expense')),
                               BankAccountId          TEXT NOT NULL,
                               AmountMinor            INTEGER NOT NULL,
                               CurrencyCode           TEXT NOT NULL CHECK (CurrencyCode IN ('USD','RUB')),
                               DateOfOperation        TEXT NOT NULL,   -- ISO-8601 с часовым поясом
                               Description            TEXT NOT NULL,
                               CategoryId             TEXT NOT NULL,
                           
                               FOREIGN KEY (BankAccountId) REFERENCES BankAccount(Id) ON DELETE CASCADE,
                               FOREIGN KEY (CategoryId)    REFERENCES Category(Id)    ON DELETE RESTRICT
                           );
                           """;

        await using var dbContext = _contextFactory.Create();
        await dbContext.CurrentTransaction.Connection!.ExecuteAsync(sql, null, dbContext.CurrentTransaction);

        await dbContext.CommitAsync();
    }
}