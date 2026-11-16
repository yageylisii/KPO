namespace Pos.Entities.Rubrics;

// Value Object: номер критерия, сравнивается по значению
// Номера идут по порядку 1, 2, 3, как номера автомобилей на семинарах
public readonly record struct CriterionId(int Value)
{
    public override string ToString() => $"#{Value}";
}
