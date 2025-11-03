using FluentValidation;
using KPO_HW2.Data.Services;
using KPO_HW2.Infrastructure.Abstractions;

namespace KPO_HW2.Infrastructure.Services;

internal sealed class AccountService : IAccountService
{
    public AccountService(AppDbContext ctx, IValidator<BankAccount> validator)
    {
        _ctx = ctx;
        _validator = validator;
    }

    private static readonly CurrencyCode DefaultCurrency = CurrencyCode.Rub;

    private readonly AppDbContext _ctx;
    private readonly IValidator<BankAccount> _validator;

    public async Task<BankAccountId> CreateAccount(string name, decimal initialBalance)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        var balance = Money.Create(
            amount: initialBalance,
            code: DefaultCurrency);

        var entity = new BankAccount
        {
            Id = BankAccountId.New(),
            Name = name,
            Balance = balance
        };

        var result = _validator.Validate(entity);
        if (!result.IsValid)
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", result.Errors)}");

        await _ctx.BankAccountRepository.AddAsync(entity);
        await _ctx.CommitAsync();

        return entity.Id;
    }

    public async Task UpdateAccount(BankAccountId id, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("newName is required.", nameof(newName));

        var existing = await _ctx.BankAccountRepository.GetByIdAsync(id)
                       ?? throw new InvalidOperationException($"Account {id} not found.");

        var updated = new BankAccount
        {
            Id = existing.Id,
            Name = newName,
            Balance = existing.Balance
        };

        var result = _validator.Validate(updated);
        if (!result.IsValid)
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", result.Errors)}");

        await _ctx.BankAccountRepository.UpdateAsync(updated);
        await _ctx.CommitAsync();
    }

    public async Task DeleteAccount(BankAccountId id)
    {
        var ok = await _ctx.BankAccountRepository.DeleteAsync(id);
        if (!ok)
            throw new InvalidOperationException($"Account {id} not found or not deleted.");

        await _ctx.CommitAsync();
    }
}