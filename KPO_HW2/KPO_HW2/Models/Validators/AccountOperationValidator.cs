using FluentValidation;

namespace KPO_HW2.Models.Validators;

public class AccountOperationValidator : AbstractValidator<AccountOperation>
{
    public AccountOperationValidator()
    {
        RuleFor(x => x.Amount.AmountMinor)
            .GreaterThan(0)
            .WithMessage("Amount must be positive");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Description must not be empty and should be less than 500 characters");

        RuleFor(x => x.BankAccountId)
            .NotEqual(default(BankAccountId))
            .WithMessage("Bank account ID must be specified");

        RuleFor(x => x.CategoryId)
            .NotEqual(default(CategoryId))
            .WithMessage("Category ID must be specified");
    }
}
