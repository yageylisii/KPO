using System.Globalization;
using Pos.UseCases.Dto;
using Pos.UseCases.Ports;

namespace Pos.Infrastructure.Export;

// Адаптер порта IFinalGradeExporter: ведомость итогов в CSV
// Колонки: студент, балл по каждому критерию, итог
public sealed class CsvFinalGradeExporter : IFinalGradeExporter
{
    private const char Separator = ';'; // Excel в русской локали ожидает ;

    public string Format => "csv";

    public void Export(IReadOnlyList<FinalGradeDto> grades, TextWriter writer)
    {
        var criteria = grades.FirstOrDefault()?.Breakdown.Select(b => b.CriterionName).ToList() ?? new();

        var header = new List<string> { "Студент" };
        header.AddRange(criteria);
        header.Add("Итог");
        header.Add("Правило");
        writer.WriteLine(string.Join(Separator, header.Select(Escape)));

        foreach (var grade in grades)
        {
            var row = new List<string> { grade.StudentName };
            row.AddRange(grade.Breakdown.Select(b => b.Score is null ? "" : Number(b.Score.Value)));
            row.Add(Number(grade.Value));
            row.Add(grade.PolicyName);
            writer.WriteLine(string.Join(Separator, row.Select(Escape)));
        }
    }

    private static string Number(decimal value) => value.ToString("0.##", CultureInfo.InvariantCulture);

    private static string Escape(string value) =>
        value.IndexOfAny(new[] { Separator, '"', '\n' }) >= 0
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
}
