using System.Text.Json;
using KPO_HW2.Infrastructure.Models;

namespace KPO_HW2.Infrastructure.DataExport;

internal class JsonExportImportFormat : ExportImportFormatBase
{
    protected override async Task<ExportImportModel> ReadAsyncCore(Stream stream)
    {
        return await JsonSerializer.DeserializeAsync<ExportImportModel>(stream) ?? new ExportImportModel([], [], []);
    }

    protected override Task WriteAsyncCore(Stream stream, ExportImportModel model)
    {
        return JsonSerializer.SerializeAsync(stream, model);
    }
}