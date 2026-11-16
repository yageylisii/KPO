using Pos.Entities.Evaluations;
using Pos.Entities.Rubrics;
using Pos.Entities.Students;
using Pos.UseCases.Ports;

namespace Pos.Infrastructure.Persistence;

// Адаптер порта IEvaluationRepository: хранение в памяти
public sealed class InMemoryEvaluationRepository : IEvaluationRepository
{
    private readonly Dictionary<EvaluationId, Evaluation> _evaluations = new();

    public EvaluationId NextId() =>
        new(_evaluations.Keys.Select(id => id.Value).DefaultIfEmpty(0).Max() + 1);

    public void Add(Evaluation evaluation) => _evaluations.Add(evaluation.Id, evaluation);

    // В памяти объект уже изменён по ссылке; метод нужен, чтобы контракт
    // работал и для файлового/БД-хранилища
    public void Update(Evaluation evaluation) => _evaluations[evaluation.Id] = evaluation;

    public void Remove(EvaluationId id) => _evaluations.Remove(id);

    public Evaluation? Find(EvaluationId id) => _evaluations.GetValueOrDefault(id);

    public Evaluation? Find(StudentId studentId, CriterionId criterionId) =>
        _evaluations.Values.FirstOrDefault(e => e.StudentId == studentId && e.CriterionId == criterionId);

    public IReadOnlyList<Evaluation> GetByStudent(StudentId studentId) =>
        _evaluations.Values.Where(e => e.StudentId == studentId).ToList();

    public IReadOnlyList<Evaluation> GetAll() => _evaluations.Values.ToList();

    public bool AnyForCriterion(CriterionId criterionId) =>
        _evaluations.Values.Any(e => e.CriterionId == criterionId);
}
