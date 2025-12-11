using System.Net;

namespace KPO_HW3.Api.Infrastructure.Exceptions;

public sealed class DownstreamHttpException(HttpStatusCode statusCode, string? body)
    : Exception($"Downstream HTTP error: {(int)statusCode} {statusCode}")
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string? Body { get; } = body;
}