using StronglyTypedIds;

namespace KPO_HW2.Models;

[StronglyTypedId(Template.Guid, "guid-dapper", "guid-yaml")]
public readonly partial struct AccountOperationId;

public class AccountOperation
{
    public required AccountOperationId Id { get; init; }
    public required CategoryId CategoryId { get; init; }
    public required BankAccountId BankAccountId { get; init; }
    public required Money Amount { get; set; }
    public required DateTimeOffset DateOfOperation { get; set; }
    public required string Description { get; set; }
}