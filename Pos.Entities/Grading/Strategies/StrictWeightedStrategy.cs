namespace Pos.Entities.Grading.Strategies;

// Итог = Σ(балл / max × вес) / Σ вес × 10 по ВСЕМ критериям рубрикатора
// Невыставленная оценка считается нулём — правило для финальной ведомости
public sealed class StrictWeightedStrategy : IGradingStrategy
{
    public string Name => "strict";

    public string Description => "все критерии, нет оценки = 0";

    public decimal Calculate(IReadOnlyList<CriterionResult> results)
    {
        var totalWeight = results.Sum(r => r.Criterion.Weight.Value);

        if (totalWeight == 0)
        {
            return 0m;
        }

        var weighted = results.Sum(r => r.Ratio * r.Criterion.Weight.Value);
        return weighted / totalWeight * FinalGrade.Scale;
    }
}
