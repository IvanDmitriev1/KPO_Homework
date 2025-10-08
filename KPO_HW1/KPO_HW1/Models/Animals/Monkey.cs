namespace KPO_HW1.Models.Animals;

internal class Monkey(HerbivoreAnimalCreateOptions options) : HerbivoreAnimal(options), IPredator
{
    public override string Species => "Monkey";
}