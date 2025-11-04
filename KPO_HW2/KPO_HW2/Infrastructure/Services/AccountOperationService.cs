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

    public Task<bool> HasOperationsWithCategory(CategoryId categoryId) =>
        _appDbContext.AccountOperationRepository.HasOperationsWithCategory(categoryId, CancellationToken.None);

    public Task<IReadOnlyList<AccountOperation>> GetByAccount(BankAccountId id)
    {
        return _appDbContext.AccountOperationRepository.GetByAccount(id, CancellationToken.None);
    }

    public async Task<AccountOperationId> CreateOperation(
        BankAccountId accountId,
        CategoryId categoryId,
        Money amount,
        DateTimeOffset date,
        string description)
    {
        var account = await _appDbContext.BankAccountRepository.GetByIdAsync(accountId)
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

        _validator.ValidateAndThrow(entity);

        account.Balance += amount;

        await _appDbContext.AccountOperationRepository.AddAsync(entity);
        await _appDbContext.BankAccountRepository.UpdateAsync(account);

        await _appDbContext.CommitAsync();
        return entity.Id;
    }

    public async Task UpdateOperation(AccountOperationId id, Money newAmount, string newDesc)
    {
        var existing = await _appDbContext.AccountOperationRepository.GetByIdAsync(id)
                       ?? throw new InvalidOperationException($"Operation {id} not found.");

        var account = await _appDbContext.BankAccountRepository.GetByIdAsync(existing.BankAccountId)
                      ?? throw new InvalidOperationException($"Account {existing.BankAccountId} not found.");

        account.Balance -= existing.Amount;

        existing.Amount = newAmount;
        existing.Description = newDesc;

        account.Balance += newAmount;

        _validator.ValidateAndThrow(existing);

        await _appDbContext.AccountOperationRepository.UpdateAsync(existing);
        await _appDbContext.BankAccountRepository.UpdateAsync(account);

        await _appDbContext.CommitAsync();
    }

    public async Task DeleteOperation(AccountOperationId id)
    {
        var existing = await _appDbContext.AccountOperationRepository.GetByIdAsync(id)
                       ?? throw new InvalidOperationException($"Operation {id} not found.");

        var account = await _appDbContext.BankAccountRepository.GetByIdAsync(existing.BankAccountId)
                      ?? throw new InvalidOperationException($"Account {existing.BankAccountId} not found.");

        account.Balance -= existing.Amount;

        var ok = await _appDbContext.AccountOperationRepository.DeleteAsync(id);
        if (!ok)
            throw new InvalidOperationException($"Operation {id} not found or not deleted.");

        await _appDbContext.BankAccountRepository.UpdateAsync(account);
        await _appDbContext.CommitAsync();
    }
}