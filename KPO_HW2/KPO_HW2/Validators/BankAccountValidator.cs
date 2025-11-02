using FluentValidation;
using KPO_HW2.Models;
using BankAccountId = KPO_HW2.Models.BankAccountId;

namespace KPO_HW2.Validators;

public class BankAccountValidator : AbstractValidator<BankAccount>
{
    public BankAccountValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage("Account name must be between 3 and 100 characters");

        RuleFor(x => x.Id)
            .NotEqual(default(BankAccountId))
            .WithMessage("Account ID must be specified");
    }
}
