using Pos.Entities.Rubrics;
using Pos.UseCases.Ports;

namespace Pos.Infrastructure.Persistence;

// Адаптер порта IRubricRepository: один рубрикатор в памяти
public sealed class InMemoryRubricRepository : IRubricRepository
{
    private Rubric _rubric = new();

    public Rubric Get() => _rubric;

    public void Save(Rubric rubric) => _rubric = rubric;
}
