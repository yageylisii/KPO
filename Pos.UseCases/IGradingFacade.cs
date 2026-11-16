using Pos.UseCases.Dto;

namespace Pos.UseCases;

// Единая точка сценариев модуля ПОС для внешнего мира (консоли, тестов,
// в будущем — веб-API). Наружу отдаются только DTO
public interface IGradingFacade
{
    // Студенты
    StudentDto AddStudent(string fullName);

    IReadOnlyList<StudentDto> GetStudents();

    // Рубрикатор
    CriterionDto AddCriterion(string name, decimal maxScore, decimal weight);

    IReadOnlyList<CriterionDto> GetCriteria();

    void RemoveCriterion(int criterionId);

    // Оценки
    EvaluationDto GiveEvaluation(int studentId, int criterionId, decimal score, string? comment);

    EvaluationDto CorrectEvaluation(int evaluationId, decimal newScore, string? comment);

    void CancelEvaluation(int evaluationId);

    IReadOnlyList<EvaluationDto> GetEvaluations(int? studentId = null);

    // Итог и экспорт
    IReadOnlyList<GradingPolicyDto> GetGradingPolicies();

    IReadOnlyList<FinalGradeDto> CalculateFinalGrades(string policyName);

    IReadOnlyList<string> GetExportFormats();

    void ExportFinalGrades(string policyName, string format, TextWriter writer);
}
