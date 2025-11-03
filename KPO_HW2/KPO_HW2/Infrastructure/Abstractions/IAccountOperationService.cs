using KPO_HW2.Data.Models;

namespace KPO_HW2.Infrastructure.Abstractions;

public interface IAccountOperationService
{
    Task<AccountOperationId> CreateOperation(
        AccountOperationId accountId,
        CategoryId categoryId,
        AccountOperationType type,
        decimal amount,
        DateTime date,
        string? description);

    Task UpdateOperation(
        AccountOperationId id,
        decimal newAmount,
        DateTime newDate,
        string? newDesc);

    Task DeleteOperation(AccountOperationId id);
}