using StronglyTypedIds;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace KPO_HW2.Data.Models;

[StronglyTypedId(Template.Guid, "guid-dapper", "guid-yaml")]
public readonly partial struct AccountOperationId;

public class AccountOperation
{
    public required AccountOperationId Id { get; init; }
    public required AccountOperationType AccountOperationType { get; init; }
    public required BankAccountId BankAccountId { get; init; }
    public required Money Amount { get; init; }
    public required DateTimeOffset DateOfOperation { get; init; }
    public required string Description { get; init; }
    public required CategoryId CategoryId { get; init; }
}