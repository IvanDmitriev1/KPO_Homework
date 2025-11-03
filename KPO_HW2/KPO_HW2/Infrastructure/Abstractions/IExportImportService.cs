using KPO_HW2.Infrastructure.DataExport;

namespace KPO_HW2.Infrastructure.Abstractions;

public interface IExportImportService
{
    Task ImportAsync(string fileName, IExportImportFormat format, CancellationToken ct = default);
    Task ExportAsync(string fileName, IExportImportFormat format, CancellationToken ct = default);
}