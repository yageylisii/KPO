using Pos.Entities.Evaluations;
using Pos.Entities.Rubrics;

namespace Pos.Entities.Grading;

// Value Object: строка расчёта итога — критерий и балл студента по нему
// (Score = null, если оценка ещё не выставлена)
public sealed record CriterionResult(Criterion Criterion, Score? Score)
{
    public bool IsEvaluated => Score is not null;

    // Доля от максимума: 7 из 10 → 0.7
    public decimal Ratio => (Score?.Value ?? 0m) / Criterion.MaxScore.Value;
}
