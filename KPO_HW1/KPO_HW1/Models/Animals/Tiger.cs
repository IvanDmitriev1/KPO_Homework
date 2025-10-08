namespace KPO_HW1.Models.Animals;

internal class Tiger(AnimalCreateOptions options) : Animal(options), IPredator
{
    public override string Species => "Tiger";
}