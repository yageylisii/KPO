namespace Pos.Entities.Grading.Strategies;

// Итог только по уже выставленным критериям — «текущая успеваемость»
// в середине курса, когда часть работ ещё не проверена
public sealed class ProgressWeightedStrategy : IGradingStrategy
{
    public string Name => "progress";

    public string Description => "только выставленные оценки";

    public decimal Calculate(IReadOnlyList<CriterionResult> results)
    {
        var evaluated = results.Where(r => r.IsEvaluated).ToList();
        var totalWeight = evaluated.Sum(r => r.Criterion.Weight.Value);

        if (totalWeight == 0)
        {
            return 0m;
        }

        var weighted = evaluated.Sum(r => r.Ratio * r.Criterion.Weight.Value);
        return weighted / totalWeight * FinalGrade.Scale;
    }
}
