using FluentValidation;
using KPO_HW2.Data.Services;
using KPO_HW2.Infrastructure.Abstractions;

namespace KPO_HW2.Infrastructure.Services;

internal class CategoryService : ICategoryService
{
    public CategoryService(AppDbContext appDbContext, IValidator<Category> validator)
    {
        _appDbContext = appDbContext;
        _validator = validator;
    }

    private readonly AppDbContext _appDbContext;
    private readonly IValidator<Category> _validator;

    public async Task<CategoryId> CreateCategory(string name, CategoryType type)
    {
        var entity = new Category
        {
            Id = CategoryId.New(),
            CategoryType = type,
            Name = name.Trim()
        };

        var result = _validator.Validate(entity);
        if (!result.IsValid)
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", result.Errors)}");

        await _appDbContext.CategoryRepository.AddAsync(entity);
        await _appDbContext.CommitAsync();

        return entity.Id;
    }

    public async Task UpdateCategory(CategoryId id, string newName)
    {
        var existing = await _appDbContext.CategoryRepository.GetByIdAsync(id)
                       ?? throw new InvalidOperationException($"Category {id} not found.");

        var updated = new Category
        {
            Id = id,
            CategoryType = existing.CategoryType,
            Name = newName.Trim()
        };

        await _appDbContext.CategoryRepository.UpdateAsync(updated);
        await _appDbContext.CommitAsync();
    }

    public async Task DeleteCategory(CategoryId id)
    {
        var ok = await _appDbContext.CategoryRepository.DeleteAsync(id);
        if (!ok)
            throw new InvalidOperationException($"Category {id} not found or not deleted.");

        await _appDbContext.CommitAsync();
    }
}