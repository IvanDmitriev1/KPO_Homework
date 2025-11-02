using StronglyTypedIds;

namespace KPO_HW2.Data.Models;

[StronglyTypedId(Template.Guid, "guid-dapper")]
public readonly partial struct CategoryId;

public class Category
{
    public required CategoryId Id { get; init; }
    public required CategoryType CategoryType { get; init; }
    public required string Name { get; init; }
}