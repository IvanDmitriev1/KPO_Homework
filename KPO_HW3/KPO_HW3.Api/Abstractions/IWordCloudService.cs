using DotNext;

namespace KPO_HW3.Api.Abstractions;

public interface IWordCloudService
{
    Task<Result<Stream>> GenerateImageForWorkAsync(
        Guid workId,
        CancellationToken ct = default);
}