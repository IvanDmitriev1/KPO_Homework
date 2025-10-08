namespace KPO_HW1.Models.Things;

internal class Computer(int number) : Thing(number)
{
    public override string Type => "Computer";
}