using Pos.Entities.Common;
using Pos.Entities.Evaluations;
using Pos.Entities.Grading;
using Pos.Entities.Rubrics;
using Pos.Entities.Students;
using Pos.UseCases.Dto;
using Pos.UseCases.Ports;

namespace Pos.UseCases;

// GoF (структурный): Facade
// без него пришлось бы создавать и работать с объектами прямо в UI
public sealed class GradingFacade : IGradingFacade
{
    private readonly IStudentRepository _students;
    private readonly IRubricRepository _rubrics;
    private readonly IEvaluationRepository _evaluations;
    private readonly EvaluationFactory _evaluationFactory;
    private readonly GradeCalculator _gradeCalculator;
    private readonly IReadOnlyList<IGradingStrategy> _strategies;
    private readonly IReadOnlyList<IFinalGradeExporter> _exporters;
    private readonly TimeProvider _time;

    public GradingFacade(
        IStudentRepository students,
        IRubricRepository rubrics,
        IEvaluationRepository evaluations,
        EvaluationFactory evaluationFactory,
        GradeCalculator gradeCalculator,
        IEnumerable<IGradingStrategy> strategies,
        IEnumerable<IFinalGradeExporter> exporters,
        TimeProvider time)
    {
        _students = students;
        _rubrics = rubrics;
        _evaluations = evaluations;
        _evaluationFactory = evaluationFactory;
        _gradeCalculator = gradeCalculator;
        _strategies = strategies.ToList();
        _exporters = exporters.ToList();
        _time = time;
    }

    private DateOnly Today => DateOnly.FromDateTime(_time.GetLocalNow().DateTime);

    // ---------- Студенты ----------

    public StudentDto AddStudent(string fullName)
    {
        var student = new Student(_students.NextId(), fullName);
        _students.Add(student);
        return ToDto(student);
    }

    public IReadOnlyList<StudentDto> GetStudents() =>
        _students.GetAll().Select(ToDto).ToList();

    // ---------- Рубрикатор ----------

    public CriterionDto AddCriterion(string name, decimal maxScore, decimal weight)
    {
        var rubric = _rubrics.Get();
        var criterion = rubric.AddCriterion(name, new MaxScore(maxScore), new Weight(weight));
        _rubrics.Save(rubric);
        return ToDto(criterion);
    }

    public IReadOnlyList<CriterionDto> GetCriteria() =>
        _rubrics.Get().Criteria.Select(ToDto).ToList();

    public void RemoveCriterion(int criterionId)
    {
        var id = new CriterionId(criterionId);

        // Правило на стыке агрегатов: удалить критерий, по которому уже есть оценки,
        // значит «осиротить» эти оценки и молча изменить итоги
        if (_evaluations.AnyForCriterion(id))
        {
            throw new DomainException("По критерию уже выставлены оценки — сначала отмените их.");
        }

        var rubric = _rubrics.Get();
        rubric.RemoveCriterion(id);
        _rubrics.Save(rubric);
    }

    // ---------- Оценки ----------

    public EvaluationDto GiveEvaluation(int studentId, int criterionId, decimal score, string? comment)
    {
        var student = GetStudent(new StudentId(studentId));
        var criterion = new CriterionId(criterionId);

        if (_evaluations.Find(student.Id, criterion) is not null)
        {
            throw new DomainException("Оценка по этому критерию уже выставлена — используйте исправление.");
        }

        var rubric = _rubrics.Get();
        var evaluation = _evaluationFactory.Create(_evaluations.NextId(), rubric, student.Id, criterion, score, Today, comment);
        _evaluations.Add(evaluation);

        return ToDto(evaluation, student, rubric);
    }

    public EvaluationDto CorrectEvaluation(int evaluationId, decimal newScore, string? comment)
    {
        var evaluation = GetEvaluation(new EvaluationId(evaluationId));
        var rubric = _rubrics.Get();
        var criterion = rubric.GetCriterion(evaluation.CriterionId);

        evaluation.Correct(criterion, new Score(newScore), Today, comment);
        _evaluations.Update(evaluation);

        return ToDto(evaluation, GetStudent(evaluation.StudentId), rubric);
    }

    public void CancelEvaluation(int evaluationId)
    {
        var evaluation = GetEvaluation(new EvaluationId(evaluationId));
        _evaluations.Remove(evaluation.Id);
    }

    public IReadOnlyList<EvaluationDto> GetEvaluations(int? studentId = null)
    {
        var rubric = _rubrics.Get();
        var evaluations = studentId is null
            ? _evaluations.GetAll()
            : _evaluations.GetByStudent(new StudentId(studentId.Value));

        return evaluations
            .Select(e => ToDto(e, GetStudent(e.StudentId), rubric))
            .ToList();
    }

    // ---------- Итог и экспорт ----------

    public IReadOnlyList<GradingPolicyDto> GetGradingPolicies() =>
        _strategies.Select(s => new GradingPolicyDto(s.Name, s.Description)).ToList();

    public IReadOnlyList<FinalGradeDto> CalculateFinalGrades(string policyName)
    {
        var strategy = _strategies.FirstOrDefault(s => s.Name == policyName)
            ?? throw new UseCaseException($"Неизвестное правило итога: {policyName}.");

        var rubric = _rubrics.Get();

        return _students.GetAll()
            .Select(student =>
            {
                var grade = _gradeCalculator.Calculate(
                    student.Id, rubric, _evaluations.GetByStudent(student.Id), strategy);
                return ToDto(grade, student);
            })
            .ToList();
    }

    public IReadOnlyList<string> GetExportFormats() =>
        _exporters.Select(e => e.Format).ToList();

    public void ExportFinalGrades(string policyName, string format, TextWriter writer)
    {
        var exporter = _exporters.FirstOrDefault(e => e.Format == format)
            ?? throw new UseCaseException($"Неизвестный формат экспорта: {format}.");

        exporter.Export(CalculateFinalGrades(policyName), writer);
    }

    // ---------- Вспомогательное ----------

    private Student GetStudent(StudentId id) =>
        _students.Find(id) ?? throw new UseCaseException($"Студент {id} не найден.");

    private Evaluation GetEvaluation(EvaluationId id) =>
        _evaluations.Find(id) ?? throw new UseCaseException($"Оценка {id} не найдена.");

    private static StudentDto ToDto(Student s) => new(s.Id.Value, s.FullName);

    private static CriterionDto ToDto(Criterion c) =>
        new(c.Id.Value, c.Name, c.MaxScore.Value, c.Weight.Value);

    private static EvaluationDto ToDto(Evaluation e, Student student, Rubric rubric)
    {
        var criterion = rubric.GetCriterion(e.CriterionId);
        return new EvaluationDto(
            e.Id.Value, student.Id.Value, student.FullName,
            criterion.Id.Value, criterion.Name,
            e.Score.Value, criterion.MaxScore.Value, e.Date, e.Comment);
    }

    private static FinalGradeDto ToDto(FinalGrade grade, Student student) =>
        new(
            student.Id.Value,
            student.FullName,
            grade.Value,
            grade.PolicyName,
            grade.Breakdown
                .Select(r => new CriterionResultDto(
                    r.Criterion.Name, r.Score?.Value, r.Criterion.MaxScore.Value, r.Criterion.Weight.Value))
                .ToList());
}
