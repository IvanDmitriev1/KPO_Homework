using KPO_HW3.Api.Infrastructure.External;
using System.Text;
using DotNext;

namespace KPO_HW3.Api.Services;

public class QuickChartWordCloudService(
    IFileStorageService fileStorage,
    HttpClient quickChartClient)
    : IWordCloudService
{
    public async Task<Result<Stream>> GenerateImageForWorkAsync(Guid workId, CancellationToken ct = default)
    {
        var fileResponseResult = await fileStorage
            .GetWorkContentResponseAsync(workId, ct);

        if (!fileResponseResult.TryGet(out var fileResponse))
            return Result.FromException<Stream>(fileResponseResult.Error!);

        using (fileResponse) ;

        await using var workStream = await fileResponse.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(
            workStream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 8192);

        var payload = new
        {
            format = "png",
            width = 800,
            height = 800,
            fontScale = 20,
            scale = "linear",
            removeStopwords = true,
            minWordLength = 3,
            text = await reader.ReadToEndAsync(ct)
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "/wordcloud");
        request.Content = JsonContent.Create(payload);

        var qcResponse = await quickChartClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            ct);

        return await HttpResponseContentStream.Create(qcResponse);
    }
}