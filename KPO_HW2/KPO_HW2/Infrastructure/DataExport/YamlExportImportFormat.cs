using KPO_HW2.Infrastructure.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KPO_HW2.Infrastructure.DataExport;

public class YamlExportImportFormat() : ExportImportFormatBase("Yaml")
{
    private static readonly IDeserializer Deserializer =
        new DeserializerBuilder()
            .WithNamingConvention(NullNamingConvention.Instance)
            .WithTypeConverter(new BankAccountId.YamlTypeConverter())
            .WithTypeConverter(new CategoryId.YamlTypeConverter())
            .WithTypeConverter(new AccountOperationId.YamlTypeConverter())
            .IgnoreUnmatchedProperties()
            .Build();

    private static readonly ISerializer Serializer =
        new SerializerBuilder()
            .WithNamingConvention(NullNamingConvention.Instance)
            .WithTypeConverter(new BankAccountId.YamlTypeConverter())
            .WithTypeConverter(new CategoryId.YamlTypeConverter())
            .WithTypeConverter(new AccountOperationId.YamlTypeConverter())
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitEmptyCollections)
            .Build();

    protected override Task<ExportImportModel> ReadAsyncCore(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var model = Deserializer.Deserialize<ExportImportModel>(reader);

        return Task.FromResult(model);
    }

    protected override async Task WriteAsyncCore(Stream stream, ExportImportModel model)
    {
        var yaml = Serializer.Serialize(model);
        await using var writer = new StreamWriter(stream);

        await writer.WriteAsync(yaml);
        await writer.FlushAsync();
    }
}