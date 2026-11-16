namespace Pos.Entities.Evaluations;

// Value Object: номер оценки, сравнивается по значению
// Номера идут по порядку 1, 2, 3, как номера автомобилей на семинарах
public readonly record struct EvaluationId(int Value)
{
    public override string ToString() => $"#{Value}";
}
