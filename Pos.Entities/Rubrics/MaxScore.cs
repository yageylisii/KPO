using Pos.Entities.Common;

namespace Pos.Entities.Rubrics;

// Value Object: максимальный балл критерия. Всегда строго больше нуля
public sealed record MaxScore
{
    public MaxScore(decimal value)
    {
        if (value <= 0)
        {
            throw new DomainException($"Максимальный балл должен быть больше 0, получено {value}.");
        }

        Value = value;
    }

    public decimal Value { get; }

    public override string ToString() => Value.ToString("0.##");
}
