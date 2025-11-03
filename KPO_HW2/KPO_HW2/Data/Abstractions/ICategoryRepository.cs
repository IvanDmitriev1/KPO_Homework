namespace KPO_HW2.Data.Abstractions;

internal interface ICategoryRepository : IRepository<Category, CategoryId>
{
    Task<IReadOnlyList<Category>> GetByType(CategoryType type);
}