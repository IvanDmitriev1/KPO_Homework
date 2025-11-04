namespace KPO_HW2.Infrastructure.Abstractions;

public interface IAccountOperationService
{
    Task<bool> HasOperationsWithCategory(CategoryId categoryId);
    Task<IReadOnlyList<AccountOperation>> GetByAccount(BankAccountId id);

    Task<AccountOperationId> CreateOperation(
        BankAccountId accountId,
        CategoryId categoryId,
        Money amount,
        DateTimeOffset date,
        string description);

    Task UpdateOperation(
        AccountOperationId id,
        Money newAmount,
        string newDesc);

    Task DeleteOperation(AccountOperationId id);
}