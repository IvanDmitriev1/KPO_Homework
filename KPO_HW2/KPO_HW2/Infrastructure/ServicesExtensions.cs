using FluentValidation;
using KPO_HW2.Infrastructure.Abstractions;
using KPO_HW2.Infrastructure.Services;
using KPO_HW2.Models.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace KPO_HW2.Infrastructure;

public static class ServicesExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        
        services.AddScoped<IValidator<BankAccount>, BankAccountValidator>();
        services.AddScoped<IValidator<Category>, CategoryValidator>();
        services.AddScoped<IValidator<AccountOperation>, AccountOperationValidator>();

        services.AddScoped<IAccountOperationService, AccountOperationService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<IExportImportService, ExportImportService>();

        return services;
    }
}