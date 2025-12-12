using System.ComponentModel.DataAnnotations;

namespace KPO_HW4.PaymentsService.Features.Accounts;

public sealed record CreateAccountResponse(AccountId AccountId, UserId UserId);

public sealed record TopUpRequest(
    [property: Required] UserId UserId,
    [property: Range(1, 100000000)] long AmountMinor);

public sealed record BalanceResponse(UserId UserId, double Balance);