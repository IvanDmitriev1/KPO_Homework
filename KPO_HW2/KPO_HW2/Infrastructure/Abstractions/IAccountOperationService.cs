namespace KPO_HW2.Infrastructure.Abstractions;

public interface IAccountOperationService
{
    Task<AccountOperationId> CreateOperation(
        BankAccountId accountId,
        CategoryId categoryId,
        Money amount,
        DateTimeOffset date,
        string description);

    Task UpdateOperation(
        AccountOperationId id,
        DateTimeOffset newDate,
        string newDesc);

    Task DeleteOperation(AccountOperationId id);
}