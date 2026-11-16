namespace Pos.Entities.Grading;

// GoF (поведенческий): Strategy — сменяемое правило подсчёта итога
// Боль: «честный итог по весам» понимают по-разному (что делать с невыставленной
// оценкой?). Правило выбирается извне, а GradeCalculator не меняется
public interface IGradingStrategy
{
    // Короткое имя для выбора в консоли и для экспорта
    string Name { get; }

    string Description { get; }

    // Возвращает итог по шкале 0..FinalGrade.Scale
    decimal Calculate(IReadOnlyList<CriterionResult> results);
}
