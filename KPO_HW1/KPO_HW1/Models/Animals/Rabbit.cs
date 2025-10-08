namespace KPO_HW1.Models.Animals;

internal class Rabbit(HerbivoreAnimalCreateOptions options) : HerbivoreAnimal(options), IPredator
{
    public override string Species => "Rabbit";
}