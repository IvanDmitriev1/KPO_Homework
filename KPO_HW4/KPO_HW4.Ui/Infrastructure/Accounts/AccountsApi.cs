namespace KPO_HW4.Ui.Infrastructure.Accounts;

public class AccountsApi(HttpClient client) : IAccountsApi
{
    public async Task<AccountBalanceResponse> GetBalanceAsync(UserId userId, CancellationToken ct = default)
    {
        return await client.GetFromJsonAsync<AccountBalanceResponse>($"balance/{userId:D}", cancellationToken: ct) ??
               throw new InvalidOperationException("Empty response body from balance endpoint.");
    }

    public async Task<AccountBalanceResponse> TopUpAsync(TopUpAccountRequest request, CancellationToken ct = default)
    {
        using var response = await client.PostAsJsonAsync("topup", request, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AccountBalanceResponse>(cancellationToken: ct) ??
               throw new InvalidOperationException("Empty response body from topup endpoint");
    }

    public async Task<List<PaymentTransactionDto>> GetTransactionsAsync(UserId userId, CancellationToken ct = default)
    {
        return await client.GetFromJsonAsync<List<PaymentTransactionDto>>($"transactions/{userId:D}",
            cancellationToken: ct) ?? [];
    }
}