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

    public Task<IReadOnlyList<BankAccount>> GetAllAccounts()
    {
        return _ctx.BankAccountRepository.GetAllAsync(CancellationToken.None);
    }

    public Task<BankAccount?> GetById(BankAccountId id)
    {
        return _ctx.BankAccountRepository.GetByIdAsync(id);
    }

    public async Task<BankAccountId> CreateAccount(string name, decimal initialBalance)
    {
        var balance = Money.Create(
            amount: initialBalance,
            code: DefaultCurrency);

        var entity = new BankAccount
        {
            Id = BankAccountId.New(),
            Name = name,
            Balance = balance
        };

        _validator.ValidateAndThrow(entity);

        await _ctx.BankAccountRepository.AddAsync(entity);
        await _ctx.CommitAsync();

        return entity.Id;
    }

    public async Task UpdateAccount(BankAccountId id, string newName)
    {
        var existing = await _ctx.BankAccountRepository.GetByIdAsync(id)
                       ?? throw new InvalidOperationException($"Account {id} not found.");

        existing.Name = newName;

        _validator.ValidateAndThrow(existing);

        await _ctx.BankAccountRepository.UpdateAsync(existing);
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