using FluentValidation;

namespace KPO_HW2.Validators;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50)
            .WithMessage("Category name must be between 2 and 50 characters");

        RuleFor(x => x.Id)
            .NotEqual(default(CategoryId))
            .WithMessage("Category ID must be specified");

        RuleFor(x => x.CategoryType)
            .IsInEnum()
            .WithMessage("Invalid category type");
    }
}