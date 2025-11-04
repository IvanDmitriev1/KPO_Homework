namespace KPO_HW2.Data.Abstractions;

internal interface IAccountOperationRepository : IRepository<AccountOperation, AccountOperationId>
{
    Task<IReadOnlyList<AccountOperation>> GetByAccount(BankAccountId accountId, CancellationToken ct);
    Task<bool> HasOperationsWithCategory(CategoryId categoryId, CancellationToken ct);
}