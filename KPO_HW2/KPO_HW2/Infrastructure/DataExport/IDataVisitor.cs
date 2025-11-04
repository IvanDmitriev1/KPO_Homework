namespace KPO_HW2.Infrastructure.DataExport;

public interface IDataVisitor
{
    void Visit(BankAccount account);
    void Visit(Category category);
    void Visit(AccountOperation operation);
}