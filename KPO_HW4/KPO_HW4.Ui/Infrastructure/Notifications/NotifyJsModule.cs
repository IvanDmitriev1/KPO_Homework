using KPO_HW4.Ui.Abstractions.Notifications;
using Microsoft.JSInterop;

namespace KPO_HW4.Ui.Infrastructure.Notifications;

public sealed class NotifyJsModule(IJSRuntime js) : INotifyService, IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask = new(() =>
        js.InvokeAsync<IJSObjectReference>("import", "./notify.js").AsTask());

    public async ValueTask<NotificationPermission> GetPermissionAsync()
    {
        var module = await _moduleTask.Value;
        var raw = await module.InvokeAsync<string>("currentPermission");
        return MapPermission(raw);
    }

    public async ValueTask<NotificationPermission> RequestPermissionAsync()
    {
        var module = await _moduleTask.Value;
        var raw = await module.InvokeAsync<string>("requestPermission");
        return MapPermission(raw);
    }

    public async ValueTask<NotifyResult> ShowAsync(string title, string body)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<NotifyResult>("show", title, body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    private static NotificationPermission MapPermission(string raw) => raw switch
    {
        "unsupported" => NotificationPermission.Unsupported,
        "default" => NotificationPermission.Default,
        "granted" => NotificationPermission.Granted,
        "denied" => NotificationPermission.Denied,
        _ => NotificationPermission.Default
    };
}
