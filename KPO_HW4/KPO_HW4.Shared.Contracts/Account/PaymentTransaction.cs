namespace KPO_HW4.Shared.Contracts.Account;

public enum PaymentTransactionType : short
{
    TopUp = 0,
    Debit
}
public enum PaymentTransactionStatus : short
{
    Processing = 0,
    Succeeded,
    Failed
}