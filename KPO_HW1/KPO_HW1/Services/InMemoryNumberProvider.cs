namespace KPO_HW1.Services;

internal class InMemoryNumberProvider : IInventoryNumberProvider
{
    private int _last;

    public int Next()
    {
        return ++_last;
    }
}