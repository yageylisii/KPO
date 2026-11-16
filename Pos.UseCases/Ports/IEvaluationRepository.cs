using Pos.Entities.Evaluations;
using Pos.Entities.Rubrics;
using Pos.Entities.Students;

namespace Pos.UseCases.Ports;

// Порт (Repository): хранение оценок снаружи домена
public interface IEvaluationRepository
{
    // Номер для нового объекта: максимальный из сохранённых + 1
    EvaluationId NextId();

    void Add(Evaluation evaluation);

    void Update(Evaluation evaluation);

    void Remove(EvaluationId id);

    Evaluation? Find(EvaluationId id);

    Evaluation? Find(StudentId studentId, CriterionId criterionId);

    IReadOnlyList<Evaluation> GetByStudent(StudentId studentId);

    IReadOnlyList<Evaluation> GetAll();

    bool AnyForCriterion(CriterionId criterionId);
}
