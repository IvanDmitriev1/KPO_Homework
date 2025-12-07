using System.Net;
using DotNext;
using KPO_HW3.Api.Infrastructure.Exceptions;

namespace KPO_HW3.Api.Infrastructure.Extensions;


public static class HttpResponseMessageStreamResultExtensions
{
    public static async Task<Result<T>> ToJsonResultAsync<T>(
        this Task<HttpResponseMessage> responseTask,
        Func<HttpStatusCode, string, Exception>? mapError = null,
        CancellationToken ct = default)
    {
        using var response = await responseTask;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            var mappedException = mapError?.Invoke(response.StatusCode, body) ??
                                  new DownstreamHttpException(response.StatusCode, body);
            return Result.FromException<T>(mappedException);
        }

        var value = await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
        if (value is null)
        {
            return Result.FromException<T>(
                new InvalidOperationException(
                    $"Empty response content for {response.RequestMessage?.RequestUri}"));
        }

        return Result.FromValue(value);
    }
}