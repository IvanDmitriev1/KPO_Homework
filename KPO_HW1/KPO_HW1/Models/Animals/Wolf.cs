namespace KPO_HW1.Models.Animals;

internal class Wolf(AnimalCreateOptions options) : Animal(options), IPredator
{
    public override string Species => "Wolf";
}