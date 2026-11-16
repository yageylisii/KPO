using Pos.Entities.Common;
using Pos.Entities.Evaluations;

namespace Pos.Entities.Rubrics;

// Entity «критерий» — за что ставят балл. Живёт внутри агрегата Rubric:
// создать критерий в обход рубрикатора нельзя (конструктор internal)
public sealed class Criterion
{
    internal Criterion(CriterionId id, string name, MaxScore maxScore, Weight weight)
    {
        Id = id;
        Name = Guard.NotBlank(name, "Название критерия");
        MaxScore = maxScore;
        Weight = weight;
    }

    public CriterionId Id { get; }

    public string Name { get; }

    public MaxScore MaxScore { get; }

    public Weight Weight { get; }

    public bool Allows(Score score) => score.Value <= MaxScore.Value;

    // Инвариант «балл не выходит за пределы критерия»
    public void EnsureAllows(Score score)
    {
        if (!Allows(score))
        {
            throw new DomainException(
                $"Балл {score} вне диапазона [0; {MaxScore}] критерия «{Name}».");
        }
    }

    public override string ToString() => $"{Name} (max {MaxScore}, вес {Weight})";
}
