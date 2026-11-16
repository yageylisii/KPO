using Pos.Entities.Rubrics;
using Pos.Entities.Students;

namespace Pos.Entities.Evaluations;

// GoF (порождающий): фабрика — единственная точка создания Evaluation
// Боль: правило «балл в пределах критерия, и критерий есть в рубрикаторе»
// касается двух агрегатов сразу, поэтому его нельзя проверить в конструкторе оценки
// Фабрика получает рубрикатор и не даёт родиться некорректной оценке
public sealed class EvaluationFactory
{
    public Evaluation Create(
        EvaluationId id,
        Rubric rubric,
        StudentId studentId,
        CriterionId criterionId,
        decimal rawScore,
        DateOnly date,
        string? comment = null)
    {
        var criterion = rubric.GetCriterion(criterionId); // критерий есть в рубрикаторе
        var score = new Score(rawScore);                  // балл не отрицательный
        criterion.EnsureAllows(score);                    // балл не больше максимума

        return new Evaluation(
            id,
            studentId,
            criterionId,
            score,
            date,
            string.IsNullOrWhiteSpace(comment) ? null : comment.Trim());
    }
}
