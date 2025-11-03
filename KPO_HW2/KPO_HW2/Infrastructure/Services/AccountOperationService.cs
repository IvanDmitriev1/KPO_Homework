using FluentValidation;
using KPO_HW2.Data.Services;
using KPO_HW2.Infrastructure.Abstractions;

namespace KPO_HW2.Infrastructure.Services;

internal class AccountOperationService : IAccountOperationService
{
    public AccountOperationService(AppDbContext appDbContext, IValidator<AccountOperation> validator)
    {
        _appDbContext = appDbContext;
        _validator = validator;
    }

    private readonly AppDbContext _appDbContext;
    private readonly IValidator<AccountOperation> _validator;

    public async Task<AccountOperationId> CreateOperation(
        BankAccountId accountId,
        CategoryId categoryId,
        Money amount,
        DateTimeOffset date,
        string description)
    {
        _ = await _appDbContext.BankAccountRepository.GetByIdAsync(accountId)
                      ?? throw new InvalidOperationException($"Account {accountId} not found.");

        _ = await _appDbContext.CategoryRepository.GetByIdAsync(categoryId)
                       ?? throw new InvalidOperationException($"Category {categoryId} not found.");

        var entity = new AccountOperation
        {
            Id = AccountOperationId.New(),
            BankAccountId = accountId,
            Amount = amount,
            DateOfOperation = date,
            Description = description.Trim(),
            CategoryId = categoryId
        };

        var result = _validator.Validate(entity);
        if (!result.IsValid)
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", result.Errors)}");

        await _appDbContext.AccountOperationRepository.AddAsync(entity);
        await _appDbContext.CommitAsync();
        return entity.Id;
    }

    public async Task UpdateOperation(AccountOperationId id, DateTimeOffset newDate, string newDesc)
    {
        var existing = await _appDbContext.AccountOperationRepository.GetByIdAsync(id)
                       ?? throw new InvalidOperationException($"Operation {id} not found.");

        var updated = new AccountOperation
        {
            Id = existing.Id,
            BankAccountId = existing.BankAccountId,
            Amount = existing.Amount,
            DateOfOperation = newDate,
            Description = newDesc.Trim(),
            CategoryId = existing.CategoryId
        };

        var result = _validator.Validate(updated);
        if (!result.IsValid)
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", result.Errors)}");

        await _appDbContext.AccountOperationRepository.UpdateAsync(updated);
        await _appDbContext.CommitAsync();
    }

    public async Task DeleteOperation(AccountOperationId id)
    {
        var ok = await _appDbContext.AccountOperationRepository.DeleteAsync(id);
        if (!ok)
            throw new InvalidOperationException($"Operation {id} not found or not deleted.");

        await _appDbContext.CommitAsync();
    }
}