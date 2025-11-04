namespace KPO_HW2.Infrastructure.Abstractions;

public interface IAccountService
{
    Task<IReadOnlyList<BankAccount>> GetAllAccounts();
    Task<BankAccount?> GetById(BankAccountId id);
    Task<BankAccountId> CreateAccount(string name, decimal initialBalance);
    Task UpdateAccount(BankAccountId id, string newName);
    Task DeleteAccount(BankAccountId id);
}