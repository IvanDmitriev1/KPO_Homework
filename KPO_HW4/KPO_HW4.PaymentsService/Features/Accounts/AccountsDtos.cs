using System.ComponentModel.DataAnnotations;
using KPO_HW4.Shared.Contracts;

namespace KPO_HW4.PaymentsService.Features.Accounts;

public sealed record CreateAccountResponse(AccountId AccountId, UserId UserId);

public sealed record TopUpRequest(
    [property: Required] UserId UserId,
    [property: Range(1, 10000000)] long AmountMinor);

public sealed record BalanceResponse(UserId UserId, double Balance);