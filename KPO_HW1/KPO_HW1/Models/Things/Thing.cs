namespace KPO_HW1.Models.Things;

internal abstract class Thing : IInventory
{
    protected Thing(int number)
    {
        Number = number;
    }

    public abstract string Type { get; }

    public int Number { get; }
}