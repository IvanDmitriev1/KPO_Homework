namespace KPO_HW2.Exceptions;

public sealed class DuplicateAccountNameException(string accountName, Exception? inner = null)
    : Exception($"Account name '{accountName}' already exists.", inner)
{
    public string AccountName { get; } = accountName;
}
