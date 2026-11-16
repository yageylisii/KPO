using Pos.UseCases.Dto;

namespace Pos.UseCases.Ports;

// Порт экспорта итогов. Сценарий знает только «выгрузить ведомость»,
// а про CSV знает адаптер в Infrastructure
public interface IFinalGradeExporter
{
    // Формат для меню и расширение файла, например "csv"
    string Format { get; }

    void Export(IReadOnlyList<FinalGradeDto> grades, TextWriter writer);
}
