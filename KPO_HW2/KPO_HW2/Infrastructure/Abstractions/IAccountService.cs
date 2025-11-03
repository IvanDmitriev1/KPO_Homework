using KPO_HW2.Data.Models;

namespace KPO_HW2.Infrastructure.Abstractions;

public interface IAccountService
{
    Task<BankAccountId> CreateAccount(string name, decimal initialBalance);
    Task UpdateAccount(BankAccountId id, string newName);
    Task DeleteAccount(BankAccountId id);
}