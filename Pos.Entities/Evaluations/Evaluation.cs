using Pos.Entities.Common;
using Pos.Entities.Rubrics;
using Pos.Entities.Students;

namespace Pos.Entities.Evaluations;

// Entity оценка — факт этому студенту по этому критерию поставили такой балл
// Создаётся только через EvaluationFactory
public sealed class Evaluation
{
    internal Evaluation(
        EvaluationId id,
        StudentId studentId,
        CriterionId criterionId,
        Score score,
        DateOnly date,
        string? comment)
    {
        Id = id;
        StudentId = studentId;
        CriterionId = criterionId;
        Score = score;
        Date = date;
        Comment = comment;
    }

    public EvaluationId Id { get; }

    public StudentId StudentId { get; }

    public CriterionId CriterionId { get; }

    public Score Score { get; private set; }

    public DateOnly Date { get; private set; }

    public string? Comment { get; private set; }

    // Исправление оценки. Требует критерий, чтобы снова проверить диапазон:
    // исправить 8 на 12 при максимуме 10 тоже нельзя
    public void Correct(Criterion criterion, Score newScore, DateOnly date, string? comment)
    {
        if (criterion.Id != CriterionId)
        {
            throw new DomainException("Оценку можно исправить только по её собственному критерию.");
        }

        criterion.EnsureAllows(newScore);

        Score = newScore;
        Date = date;
        Comment = string.IsNullOrWhiteSpace(comment) ? Comment : comment.Trim();
    }
}
