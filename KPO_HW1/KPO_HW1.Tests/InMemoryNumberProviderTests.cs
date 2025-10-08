namespace KPO_HW1.Tests;

public class InMemoryNumberProviderTests
{
    [Fact]
    public void Next_returns_incrementing_sequence_starting_at_1()
    {
        var gen = new InMemoryNumberProvider();
        Assert.Equal(1, gen.Next());
        Assert.Equal(2, gen.Next());
        Assert.Equal(3, gen.Next());
    }
}