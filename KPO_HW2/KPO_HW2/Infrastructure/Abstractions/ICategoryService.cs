namespace KPO_HW2.Infrastructure.Abstractions;

public interface ICategoryService
{
    Task<IReadOnlyList<Category>> GetAll();
    Task<CategoryId> CreateCategory(string name, CategoryType type);
    Task UpdateCategory(CategoryId id, string newName);
    Task DeleteCategory(CategoryId id);
}