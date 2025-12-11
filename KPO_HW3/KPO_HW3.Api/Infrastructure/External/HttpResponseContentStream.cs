namespace KPO_HW3.Api.Infrastructure.External;

/// <summary>
/// Обёртка над Stream, которая держит HttpResponseMessage живым,
/// а при Dispose освобождает оба ресурса.
/// </summary>
internal sealed class HttpResponseContentStream : Stream
{
    public static async Task<HttpResponseContentStream> Create(HttpResponseMessage response)
    {
        var stream = await response.Content.ReadAsStreamAsync();
        return new HttpResponseContentStream(stream, response);
    }

    private HttpResponseContentStream(Stream inner, HttpResponseMessage response)
    {
        _inner = inner;
        _response = response;
    }

    private readonly Stream _inner;
    private readonly HttpResponseMessage _response;

    public override bool CanRead => _inner.CanRead;
    public override bool CanSeek => _inner.CanSeek;
    public override bool CanWrite => _inner.CanWrite;
    public override long Length => _inner.Length;

    public override long Position
    {
        get => _inner.Position;
        set => _inner.Position = value;
    }

    public override void Flush() => _inner.Flush();

    public override int Read(byte[] buffer, int offset, int count)
        => _inner.Read(buffer, offset, count);

    public override Task<int> ReadAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken)
        => _inner.ReadAsync(buffer, offset, count, cancellationToken);

    public override long Seek(long offset, SeekOrigin origin)
        => _inner.Seek(offset, origin);

    public override void SetLength(long value)
        => _inner.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
        => _inner.Write(buffer, offset, count);

    public override ValueTask DisposeAsync()
    {
        _response.Dispose();
        return ValueTask.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        _response.Dispose();
    }
}