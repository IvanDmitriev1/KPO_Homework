namespace KPO_HW2.Data.Abstractions;

internal interface IBankAccountRepository : IRepository<BankAccount, BankAccountId>
{
    Task<BankAccount?> GetByNameAsync(string name, CancellationToken ct = default);
}