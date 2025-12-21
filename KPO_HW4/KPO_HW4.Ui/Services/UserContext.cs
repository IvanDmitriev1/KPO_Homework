using Blazored.LocalStorage;

namespace KPO_HW4.Ui.Services;

public class UserContext(ILocalStorageService localStorageService) : IUserContext, IDisposable
{
    private const string StorageKey = "userId";
    private readonly SemaphoreSlim _lock = new(1, 1);

    public UserId UserId { get; private set; } = UserId.Empty;

    public async Task Initialize(CancellationToken ct)
    {
        if (UserId != UserId.Empty)
            return;

        await _lock.WaitAsync(ct);

        try
        {
            UserId = await localStorageService.GetItemAsync<UserId>(StorageKey, ct);
            if (UserId == UserId.Empty)
            {
                UserId = UserId.New();
                await localStorageService.SetItemAsync(StorageKey, UserId, ct);
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose()
    {
        _lock.Dispose();
    }
}