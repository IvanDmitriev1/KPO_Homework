using KPO_HW2.Data.Models;
using KPO_HW2.Infrastructure.DataExport;

namespace KPO_HW2.Infrastructure.Models;

public record ExportImportModel(
    IReadOnlyList<BankAccount> Accounts,
    IReadOnlyList<Category> Categories,
    IReadOnlyList<AccountOperation> AccountOperations)
{
    public void Accept(IDataVisitor visitor)
    {
        foreach (var account in Accounts)
            visitor.Visit(account);

        foreach (var category in Categories)
            visitor.Visit(category);

        foreach (var operation in AccountOperations)
            visitor.Visit(operation);
    }
}