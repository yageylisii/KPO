using Pos.Entities.Common;

namespace Pos.Entities.Rubrics;

// Value Object: вклад критерия в итог. Вес относительный:
// итог нормируется на сумму весов, поэтому веса не обязаны давать в сумме 1
public sealed record Weight
{
    public Weight(decimal value)
    {
        if (value <= 0)
        {
            throw new DomainException($"Вес критерия должен быть больше 0, получено {value}.");
        }

        Value = value;
    }

    public decimal Value { get; }

    public override string ToString() => Value.ToString("0.##");
}
