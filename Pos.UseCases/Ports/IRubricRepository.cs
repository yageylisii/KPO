using Pos.Entities.Rubrics;

namespace Pos.UseCases.Ports;

// Порт (Repository): рубрикатор курса. Сейчас курс один — и рубрикатор один
public interface IRubricRepository
{
    Rubric Get();

    void Save(Rubric rubric);
}
