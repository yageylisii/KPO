namespace Pos.Entities.Students;

// Value Object: номер студента, сравнивается по значению
// Номера идут по порядку 1, 2, 3
public readonly record struct StudentId(int Value)
{
    public override string ToString() => $"#{Value}";
}
