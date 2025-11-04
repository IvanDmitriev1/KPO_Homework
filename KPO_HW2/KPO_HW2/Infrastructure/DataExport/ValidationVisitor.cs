using FluentValidation;

namespace KPO_HW2.Infrastructure.DataExport;

internal class ValidationVisitor : IDataVisitor
{
    public ValidationVisitor(
        IValidator<BankAccount> bankAccountValidator,
        IValidator<Category> categoryValidator,
        IValidator<AccountOperation> accountOperationValidator)
    {
        _bankAccountValidator = bankAccountValidator;
        _categoryValidator = categoryValidator;
        _accountOperationValidator = accountOperationValidator;
    }

    private readonly IValidator<BankAccount> _bankAccountValidator;
    private readonly IValidator<Category> _categoryValidator;
    private readonly IValidator<AccountOperation> _accountOperationValidator;
    public List<string> Errors { get; } = [];

    public bool IsValid => Errors.Count == 0;

    public void Visit(BankAccount account)
    {
        var result = _bankAccountValidator.Validate(account);
        Errors.AddRange(result.Errors.Select(failure => failure.ErrorMessage));
    }

    public void Visit(Category category)
    {
        var result = _categoryValidator.Validate(category);
        Errors.AddRange(result.Errors.Select(failure => failure.ErrorMessage));
    }

    public void Visit(AccountOperation operation)
    {
        var result = _accountOperationValidator.Validate(operation);
        Errors.AddRange(result.Errors.Select(failure => failure.ErrorMessage));
    }
}