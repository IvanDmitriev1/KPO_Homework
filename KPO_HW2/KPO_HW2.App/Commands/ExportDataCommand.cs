using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.Infrastructure.Abstractions;
using KPO_HW2.Infrastructure.DataExport;
using Spectre.Console;

namespace KPO_HW2.App.Commands;

internal class ExportDataCommand(IExportImportService service) : ICommand
{
    public string Name => "Экспорт данных в файл";

    private readonly IExportImportFormat[] _formats = [new JsonExportImportFormat(), new YamlExportImportFormat()];

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var format = AnsiConsole.Prompt(
            new SelectionPrompt<IExportImportFormat>()
                .Title("Выберите [green]формат файла[/] для экспорта:")
                .UseConverter(f => f.Name)
                .AddChoices(_formats));

        var fileName = AnsiConsole.Ask<string>(
            $"Введите [green]путь к файлу[/]");

        if (File.Exists(fileName))
        {
            var overwrite = AnsiConsole.Confirm(
                $"Файл [yellow]{fileName}[/] уже существует. [red]Перезаписать[/]?");
            if (!overwrite)
            {
                AnsiConsole.MarkupLine("[yellow]Экспорт отменён пользователем.[/]");
                return;
            }
        }

        AnsiConsole.MarkupLine(
            $"Экспорт в файл [yellow]{fileName}[/] в формате [yellow]{format.Name}[/]...");

        await service.ExportAsync(fileName, format, ct);
        AnsiConsole.MarkupLine("[green]Экспорт завершён успешно.[/]");
    }
}