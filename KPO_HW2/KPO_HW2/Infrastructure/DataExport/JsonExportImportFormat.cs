using System.Text.Json;
using KPO_HW2.Infrastructure.Models;

namespace KPO_HW2.Infrastructure.DataExport;

public class JsonExportImportFormat() : ExportImportFormatBase("Json")
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