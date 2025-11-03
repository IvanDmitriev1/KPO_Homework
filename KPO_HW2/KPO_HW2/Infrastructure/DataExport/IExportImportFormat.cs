using KPO_HW2.Infrastructure.Models;

namespace KPO_HW2.Infrastructure.DataExport;

public interface IExportImportFormat
{
    Task<ExportImportModel> ReadAsync(string fileName, IDataVisitor? visitor = null);
    Task WriteAsync(string fileName, ExportImportModel model, IDataVisitor? visitor = null);
}