using KPO_HW4.PaymentsService.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW4.PaymentsService.AccountsFeature;

public static class AccountsEndpoints
{
    public static IEndpointRouteBuilder MapAccountsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/accounts")
            .WithTags("Accounts");

        group.MapPost("/", CreateAccountHandle);

        group.MapPost("/topup", TopUpAccountHandle)
            .DisableAntiforgery();

        group.MapGet("/balance", GetAccountBalance);

        return app;
    }

    private static async Task<Results<Created<CreateAccountResponse>, Ok<CreateAccountResponse>>> CreateAccountHandle(
        [FromQuery] UserId userId,
        [FromServices] PaymentsDbContext db,
        CancellationToken ct)
    {
        var account = await db.Accounts
            .Where(a => a.UserId == userId)
            .FirstOrDefaultAsync(ct);

        if (account is not null)
        {
            return TypedResults.Ok(new CreateAccountResponse(account.Id, account.UserId));
        }

        account = new Account
        {
            UserId = userId
        };

        db.Accounts.Add(account);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created("/payments/accounts/balance", new CreateAccountResponse(account.Id, account.UserId));
    }

    private static async Task<Results<Ok<BalanceResponse>, NotFound, BadRequest<ProblemDetails>>> TopUpAccountHandle(
        [FromForm] TopUpRequest req,
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

        var accountId = await db.Accounts
            .AsNoTracking()
            .Where(a => a.UserId == req.UserId)
            .Select(a => new { a.Id })
            .FirstOrDefaultAsync(ct);

        if (accountId is null)
        {
            return TypedResults.NotFound();
        }

        var paymentTransaction = PaymentTransaction.CreateTopUp(accountId.Id, req.AmountMinor);
        db.Transactions.Add(paymentTransaction);

        await db.Accounts
            .Where(a => a.Id == accountId.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(
                a => a.BalanceMinor,
                a => a.BalanceMinor + req.AmountMinor), ct);

        paymentTransaction.MarkSucceeded();
        await db.SaveChangesAsync(ct);
            
        var balance = await GetBalanceByUserId(db, req.UserId, ct);
        return TypedResults.Ok(new BalanceResponse(req.UserId, balance!.Value));
    }

    private static async Task<Results<Ok<BalanceResponse>, NotFound>> GetAccountBalance(
        [FromQuery] UserId userId,
        [FromServices] PaymentsDbContext db,
        CancellationToken ct)
    {
        var balance = await GetBalanceByUserId(db, userId, ct);

        if (balance is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new BalanceResponse(userId, balance.Value));
    }

    private static async Task<double?> GetBalanceByUserId(PaymentsDbContext db, UserId userId, CancellationToken ct)
    {
        var balanceMinor = await db.Accounts.AsNoTracking()
            .Where(a => a.UserId == userId)
            .Select(a => (long?)a.BalanceMinor)
            .FirstOrDefaultAsync(ct);

        return balanceMinor / 100;
    }
}