using Pos.Entities.Students;

namespace Pos.Entities.Grading;

// Value Object «итог» по 10-балльной шкале. Нигде не хранится:
// его всегда заново считает GradeCalculator из текущих оценок,
// поэтому он не может «разойтись» с оценками
public sealed record FinalGrade(
    StudentId StudentId,
    decimal Value,
    string PolicyName,
    IReadOnlyList<CriterionResult> Breakdown)
{
    public const decimal Scale = 10m;
}
