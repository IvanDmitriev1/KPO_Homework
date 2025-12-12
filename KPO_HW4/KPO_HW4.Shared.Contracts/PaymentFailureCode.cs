namespace KPO_HW4.Shared.Contracts;

public enum PaymentFailureCode : short
{
    None = 0,
    InsufficientFunds = 1,
    AccountNotFound = 2,
    InvalidAmount = 3,
    Unknown = 100
}