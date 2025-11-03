using KPO_HW2.Infrastructure.Models;

namespace KPO_HW2.Infrastructure.DataExport;

public abstract class ExportImportFormatBase : IExportImportFormat
{
    public async Task<ExportImportModel> ReadAsync(string fileName, IDataVisitor? visitor = null)
    {
        await using var stream = File.OpenRead(fileName);
        var model = await ReadAsyncCore(stream);

        if (visitor is not null)
            model.Accept(visitor);

        return model;
    }

    public async Task WriteAsync(string fileName, ExportImportModel model, IDataVisitor? visitor = null)
    {
        if (visitor is not null)
            model.Accept(visitor);

        await using var stream = File.OpenWrite(fileName);
        await WriteAsyncCore(stream, model);
    }

    protected abstract Task<ExportImportModel> ReadAsyncCore(Stream stream);
    protected abstract Task WriteAsyncCore(Stream stream, ExportImportModel model);
}