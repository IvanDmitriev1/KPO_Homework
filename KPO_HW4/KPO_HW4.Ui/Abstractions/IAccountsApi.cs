namespace KPO_HW4.Ui.Abstractions;

public interface IAccountsApi
{
    Task<AccountBalanceResponse> GetBalanceAsync(UserId userId, CancellationToken ct = default);
    Task<AccountBalanceResponse> TopUpAsync(TopUpAccountRequest request, CancellationToken ct = default);
    Task<List<PaymentTransactionDto>> GetTransactionsAsync(UserId userId, CancellationToken ct = default);
}