using FluentValidation;
using KPO_HW2.Data.Services;
using KPO_HW2.Infrastructure.Abstractions;
using KPO_HW2.Infrastructure.DataExport;
using KPO_HW2.Infrastructure.Models;

namespace KPO_HW2.Infrastructure.Services;

internal class ExportImportService : IExportImportService
{
    public ExportImportService(
        AppDbContext appDbContext,
        IValidator<BankAccount> bankAccountValidator,
        IValidator<Category> categoryValidator,
        IValidator<AccountOperation> accountOperationValidator)
    {
        _appDbContext = appDbContext;
        _bankAccountValidator = bankAccountValidator;
        _categoryValidator = categoryValidator;
        _accountOperationValidator = accountOperationValidator;
    }

    private readonly AppDbContext _appDbContext;
    private readonly IValidator<BankAccount> _bankAccountValidator;
    private readonly IValidator<Category> _categoryValidator;
    private readonly IValidator<AccountOperation> _accountOperationValidator;

    public async Task ImportAsync(string fileName, IExportImportFormat format, CancellationToken ct = default)
    {
        var validator = new ValidationVisitor(_bankAccountValidator, _categoryValidator, _accountOperationValidator);
        var data = await format.ReadAsync(fileName, validator);
        var (accounts, categories, accountOperations) = data;

        if (!validator.IsValid)
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", validator.Errors)}");

        foreach (var account in accounts)
        {
            await _appDbContext.BankAccountRepository.AddAsync(account);
        }

        foreach (var category in categories)
        {
            await _appDbContext.CategoryRepository.AddAsync(category);
        }

        foreach (var accountOperation in accountOperations)
        {
            await _appDbContext.AccountOperationRepository.AddAsync(accountOperation);
        }

        await _appDbContext.CommitAsync(ct);
    }

    public async Task ExportAsync(string fileName, IExportImportFormat format, CancellationToken ct = default)
    {
        var accounts = await _appDbContext.BankAccountRepository.GetAllAsync(ct);
        var categories = await _appDbContext.CategoryRepository.GetAllAsync(ct);
        var accountOperations = await _appDbContext.AccountOperationRepository.GetAllAsync(ct);

        await format.WriteAsync(fileName, new ExportImportModel(accounts, categories, accountOperations));
    }
}