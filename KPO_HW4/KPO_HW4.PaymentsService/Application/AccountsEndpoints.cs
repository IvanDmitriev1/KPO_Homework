using KPO_HW4.PaymentsService.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace KPO_HW4.PaymentsService.Application;

public static class AccountsEndpoints
{
    public static IEndpointRouteBuilder MapAccountsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/accounts")
            .WithTags("Accounts");

        group.MapPost("/topup", TopUpAccountHandle);
        group.MapGet("/balance/{userId}", GetAccountBalanceHandler);
        group.MapGet("/transactions/{userId}", GetTransactionsHandler);

        return app;
    }

    private static async Task<Results<Ok<AccountBalanceResponse>, BadRequest<ProblemDetails>>> TopUpAccountHandle(
        [FromBody] TopUpAccountRequest req,
        [FromServices] PaymentsDbContext db,
        CancellationToken ct)
    {
        if (req.AmountMinor <= 0)
        {
            return TypedResults.BadRequest(new ProblemDetails()
            {
                Detail = "AmountMinor must be > 0"
            });
        }

        var accountIdData = await db.Accounts
            .AsNoTracking()
            .Where(a => a.UserId == req.UserId)
            .Select(a => new { a.Id })
            .FirstOrDefaultAsync(ct);

        var accountId = accountIdData?.Id ?? await CreateAccount(req.UserId, db, ct);

        var paymentTransaction = PaymentTransaction.CreateTopUp(accountId, req.AmountMinor);
        db.Transactions.Add(paymentTransaction);

        await db.Accounts
            .Where(a => a.Id == accountId)
            .ExecuteUpdateAsync(s => s.SetProperty(
                a => a.BalanceMinor,
                a => a.BalanceMinor + req.AmountMinor), ct);

        paymentTransaction.MarkSucceeded();
        await db.SaveChangesAsync(ct);
            
        var balance = await GetBalanceByUserId(db, req.UserId, ct);
        return TypedResults.Ok(new AccountBalanceResponse(req.UserId, balance!.Value));
    }

    private static async Task<Ok<AccountBalanceResponse>> GetAccountBalanceHandler(
        [FromRoute] UserId userId,
        [FromServices] PaymentsDbContext db,
        CancellationToken ct)
    {
        var balance = await GetBalanceByUserId(db, userId, ct);

        if (balance is null)
        {
            await CreateAccount(userId, db, ct);
            balance = 0;
        }

        return TypedResults.Ok(new AccountBalanceResponse(userId, balance.Value));
    }

    private static async IAsyncEnumerable<PaymentTransactionDto> GetTransactionsHandler(
        [FromRoute] UserId userId,
        [FromServices] PaymentsDbContext db,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var query = db.Transactions
            .AsNoTracking()
            .Join(
                db.Accounts.AsNoTracking(),
                t => t.AccountId,
                a => a.Id,
                (t, a) => new { t, a }
            )
            .Where(x => x.a.UserId == userId)
            .OrderByDescending(x => x.t.CreatedAt)
            .Select(x => new PaymentTransactionDto(
                UserId: x.a.UserId,
                ReferenceId: x.t.ReferenceId,
                Type: x.t.Type,
                Amount: x.t.AmountMinor / 100m,
                Status: x.t.Status,
                FailureCode: x.t.FailureCode,
                CreatedAt: x.t.CreatedAt
            ))
            .AsAsyncEnumerable()
            .WithCancellation(ct);

        await foreach (var payment in query)
        {
            yield return payment;
        }
    }

    private static async Task<AccountId> CreateAccount(UserId userId, PaymentsDbContext db, CancellationToken ct)
    {
        var account = await db.Accounts
            .Where(a => a.UserId == userId)
            .FirstOrDefaultAsync(ct);

        if (account is not null)
        {
            return account.Id;
        }

        account = new Account
        {
            UserId = userId
        };

        db.Accounts.Add(account);
        await db.SaveChangesAsync(ct);

        return account.Id;
    }

    private static async Task<decimal?> GetBalanceByUserId(PaymentsDbContext db, UserId userId, CancellationToken ct)
    {
        var balanceMinor = await db.Accounts.AsNoTracking()
            .Where(a => a.UserId == userId)
            .Select(a => (long?)a.BalanceMinor)
            .FirstOrDefaultAsync(ct);

        return balanceMinor / 100;
    }
}