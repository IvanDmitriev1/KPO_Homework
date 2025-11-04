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

    public Task<IReadOnlyList<Category>> GetAll() => _appDbContext.CategoryRepository.GetAllAsync();

    public async Task<CategoryId> CreateCategory(string name, CategoryType type)
    {
        var entity = new Category
        {
            Id = CategoryId.New(),
            CategoryType = type,
            Name = name.Trim()
        };

        _validator.ValidateAndThrow(entity);

        await _appDbContext.CategoryRepository.AddAsync(entity);
        await _appDbContext.CommitAsync();

        return entity.Id;
    }

    public async Task UpdateCategory(CategoryId id, string newName)
    {
        var existing = await _appDbContext.CategoryRepository.GetByIdAsync(id)
                       ?? throw new InvalidOperationException($"Category {id} not found.");

        existing.Name = newName.Trim();

        _validator.ValidateAndThrow(existing);

        await _appDbContext.CategoryRepository.UpdateAsync(existing);
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