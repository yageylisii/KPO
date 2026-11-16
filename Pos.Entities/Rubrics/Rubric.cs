using Pos.Entities.Common;

namespace Pos.Entities.Rubrics;

// Агрегат «рубрикатор» (aggregate root): набор критериев курса

public sealed class Rubric
{
    private readonly List<Criterion> _criteria = new();

    public IReadOnlyList<Criterion> Criteria => _criteria;

    public bool IsEmpty => _criteria.Count == 0;

    public Criterion AddCriterion(string name, MaxScore maxScore, Weight weight)
    {
        var criterion = new Criterion(NextCriterionId(), name, maxScore, weight);

        if (_criteria.Any(c => string.Equals(c.Name, criterion.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException($"Критерий «{criterion.Name}» уже есть в рубрикаторе.");
        }

        _criteria.Add(criterion);
        return criterion;
    }

    public bool Contains(CriterionId id) => _criteria.Any(c => c.Id == id);

    // Инвариант «оценку можно выставить только по критерию из рубрикатора»
    public Criterion GetCriterion(CriterionId id) =>
        _criteria.FirstOrDefault(c => c.Id == id)
        ?? throw new DomainException($"Критерия {id} нет в рубрикаторе.");

    public void RemoveCriterion(CriterionId id) => _criteria.Remove(GetCriterion(id));

    // Критерии живут внутри агрегата, поэтому номер следующего выдаёт сам рубрикатор
    // Следующий номер = максимальный + 1, как CarNumberService на семинарах 8 и 10
    private CriterionId NextCriterionId() =>
        new(_criteria.Select(c => c.Id.Value).DefaultIfEmpty(0).Max() + 1);
}
