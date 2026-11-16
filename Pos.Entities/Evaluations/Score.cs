using Pos.Entities.Common;

namespace Pos.Entities.Evaluations;

// Value Object: балл. Сам по себе знает только, что он не отрицательный;
// верхнюю границу задаёт критерий (Criterion.EnsureAllows)
public sealed record Score
{
    public Score(decimal value)
    {
        if (value < 0)
        {
            throw new DomainException($"Балл не может быть отрицательным, получено {value}.");
        }

        Value = value;
    }

    public decimal Value { get; }

    public override string ToString() => Value.ToString("0.##");
}
