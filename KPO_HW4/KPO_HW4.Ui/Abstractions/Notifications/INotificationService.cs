namespace KPO_HW4.Ui.Abstractions.Notifications;

public interface INotifyService
{
    ValueTask<NotificationPermission> GetPermissionAsync();
    ValueTask<NotificationPermission> RequestPermissionAsync();
    ValueTask<NotifyResult> ShowAsync(string title, string body);
}