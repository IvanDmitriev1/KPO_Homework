namespace KPO_HW4.Ui.Abstractions.Notifications;


public sealed record NotifyResult(bool Ok, string? Reason)
{
    public static NotifyResult Success() => new(true, null);
    public static NotifyResult Fail(string reason) => new(false, reason);
}