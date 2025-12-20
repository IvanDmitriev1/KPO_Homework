using Blazored.LocalStorage;

namespace KPO_HW4.Ui.Services;

public class UserContext(ILocalStorageService localStorageService) : IUserContext, IDisposable
{
    private const string StorageKey = "userId";
    private readonly SemaphoreSlim _lock = new(1, 1);

    private UserId? _cached;

    public async ValueTask<UserId> GetUserIdAsync(CancellationToken ct)
    {
        if (_cached is not null)
            return _cached.Value;

        await _lock.WaitAsync(ct);

        try
        {
            _cached = await localStorageService.GetItemAsync<UserId>(StorageKey, ct);
            if (_cached.Value == UserId.Empty)
            {
                _cached = UserId.New();
                await localStorageService.SetItemAsync(StorageKey, _cached.Value, ct);
            }
            
            return _cached.Value;
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