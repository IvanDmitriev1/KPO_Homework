using StronglyTypedIds;

namespace KPO_HW2.Data.Models;


[StronglyTypedId(Template.Guid, "guid-dapper")]
public readonly partial struct BankAccountId;

public class BankAccount
{
    public required BankAccountId Id { get; init; }
    public required string Name { get; init; }
    public Money Balance { get; set; }
}