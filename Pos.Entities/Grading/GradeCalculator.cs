using Pos.Entities.Common;
using Pos.Entities.Evaluations;
using Pos.Entities.Rubrics;
using Pos.Entities.Students;

namespace Pos.Entities.Grading;

// Domain Service «расчёт итога». Правило не принадлежит ни одной сущности:
// итог зависит и от рубрикатора (max, веса), и от набора оценок студента
// Сервис гарантирует, что итог согласован с текущими оценками:
// чужие оценки и оценки по критериям вне рубрикатора не принимаются
public sealed class GradeCalculator
{
    public FinalGrade Calculate(
        StudentId studentId,
        Rubric rubric,
        IEnumerable<Evaluation> evaluations,
        IGradingStrategy strategy)
    {
        if (rubric.IsEmpty)
        {
            throw new DomainException("Рубрикатор пуст: итог посчитать не из чего.");
        }

        var byCriterion = new Dictionary<CriterionId, Evaluation>();

        foreach (var evaluation in evaluations)
        {
            if (evaluation.StudentId != studentId)
            {
                throw new DomainException("В расчёт итога попала оценка другого студента.");
            }

            if (!rubric.Contains(evaluation.CriterionId))
            {
                throw new DomainException($"Оценка {evaluation.Id} выставлена по критерию вне рубрикатора.");
            }

            if (!byCriterion.TryAdd(evaluation.CriterionId, evaluation))
            {
                throw new DomainException("У студента две оценки по одному критерию.");
            }
        }

        var results = rubric.Criteria
            .Select(c => new CriterionResult(c, byCriterion.GetValueOrDefault(c.Id)?.Score))
            .ToList();

        return new FinalGrade(studentId, strategy.Calculate(results), strategy.Name, results);
    }
}
