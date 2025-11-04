using KPO_HW2.App.Commands;
using KPO_HW2.App.Commands.Account;
using KPO_HW2.App.Commands.Category;
using KPO_HW2.App.Commands.Operations;
using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.App.Middlewares;
using KPO_HW2.Data;
using KPO_HW2.Data.Abstractions;
using KPO_HW2.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;


var services = new ServiceCollection();
services.AddDb("Data Source=mydb.db;");
services.AddInfrastructure();

services.AddSingleton<CommandPipeline>();
services.AddSingleton<ICommandMiddleware, TimingMiddleware>();
services.AddSingleton<ICommandMiddleware, ValidationExceptionMiddleware>();
services.AddSingleton<ICommandMiddleware, ErrorHandlingMiddleware>();

services.AddScoped<CreateAccountCommand>();
services.AddScoped<EditAccountCommand>();
services.AddScoped<DeleteAccountCommand>();
services.AddScoped<ViewAccountDetailsCommand>();

services.AddScoped<CreateCategoryCommand>();
services.AddScoped<EditCategoryCommand>();
services.AddScoped<DeleteCategoryCommand>();

services.AddScoped<CreateOperationCommand>();
services.AddScoped<EditOperationCommand>();
services.AddScoped<DeleteOperationCommand>();

services.AddScoped<ImportDataCommand>();
services.AddScoped<ExportDataCommand>();

var sp = services.BuildServiceProvider();

var dbInitializer = sp.GetRequiredService<IDbInitializer>();
await dbInitializer.Initialize();

var menuItems = new List<CommandDescriptor>
{
    new("Создать счёт", typeof(CreateAccountCommand)),
    new("Редактировать счёт", typeof(EditAccountCommand)),
    new("Удалить счёт", typeof(DeleteAccountCommand)),
    new("Посмотреть баланс и операции по счёту", typeof(ViewAccountDetailsCommand)),

    new("Создать категорию", typeof(CreateCategoryCommand)),
    new("Редактировать категорию", typeof(EditCategoryCommand)),
    new("Удалить категорию", typeof(DeleteCategoryCommand)),

    new("Создать операцию", typeof(CreateOperationCommand)),
    new("Редактировать операцию", typeof(EditOperationCommand)),
    new("Удалить операцию", typeof(DeleteOperationCommand)),

    new("Импорт данных из файла", typeof(ImportDataCommand)),
    new("Экспорт данных в файл", typeof(ExportDataCommand)),

};


var pipeline = sp.GetRequiredService<CommandPipeline>();


while (true)
{
    AnsiConsole.Clear();

    var selected = AnsiConsole.Prompt(
        new SelectionPrompt<CommandDescriptor>()
            .Title("Выберите [green]операцию[/]:")
            .UseConverter(c => c.Name)
            .AddChoices(menuItems)
            .PageSize(15));

    await pipeline.ExecuteAsync(selected, CancellationToken.None);

    if (!AnsiConsole.Confirm("Продолжить?"))
        break;
}
