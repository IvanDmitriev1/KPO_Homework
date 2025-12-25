using Microsoft.AspNetCore.Components;

namespace KPO_HW4.Ui.Components.Common;

public abstract class CancellableComponentBase : ComponentBase, IDisposable
{
    private readonly CancellationTokenSource _cts = new();

    public CancellationToken CancellationToken => _cts.Token;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}