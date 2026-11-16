using Pos.Entities.Common;
using Pos.UseCases;

namespace Pos.ConsoleApp.Ui;

// Сквозной сценарий из ТЗ: студенты и критерии - оценки - итог - экспорт
// Заодно показывает, что домен не даёт нарушить правила
public sealed class DemoScenario
{
    private readonly IGradingFacade _pos;

    public DemoScenario(IGradingFacade pos)
    {
        _pos = pos;
    }

    public void Run()
    {
        Step("1. Рубрикатор");
        var homework = _pos.AddCriterion("Домашние задания", maxScore: 10, weight: 0.4m);
        var seminars = _pos.AddCriterion("Работа на семинарах", maxScore: 5, weight: 0.2m);
        var exam = _pos.AddCriterion("Экзамен", maxScore: 10, weight: 0.4m);
        foreach (var c in _pos.GetCriteria())
        {
            Console.WriteLine($"  + {ConsoleMenu.Describe(c)}");
        }

        Step("2. Студенты");
        var ivan = _pos.AddStudent("Иванов Иван");
        var maria = _pos.AddStudent("Петрова Мария");
        Console.WriteLine($"  + {ivan.FullName}\n  + {maria.FullName}");

        Step("3. Оценки");
        _pos.GiveEvaluation(ivan.Id, homework.Id, 8, "сдал всё вовремя");
        _pos.GiveEvaluation(ivan.Id, seminars.Id, 4, null);
        _pos.GiveEvaluation(ivan.Id, exam.Id, 7, null);
        _pos.GiveEvaluation(maria.Id, homework.Id, 10, null);
        var mariaSeminars = _pos.GiveEvaluation(maria.Id, seminars.Id, 3, null);
        foreach (var e in _pos.GetEvaluations())
        {
            Console.WriteLine($"  + {ConsoleMenu.Describe(e)}");
        }

        Step("4. Попытки нарушить правила");
        TryViolation("12 из 10", () => _pos.GiveEvaluation(maria.Id, exam.Id, 12, null));
        TryViolation("отрицательный балл", () => _pos.GiveEvaluation(maria.Id, exam.Id, -1, null));
        TryViolation("критерий вне рубрикатора", () => _pos.GiveEvaluation(maria.Id, criterionId: 999, 5, null));
        TryViolation("повторная оценка", () => _pos.GiveEvaluation(ivan.Id, exam.Id, 9, null));

        Step("5. Исправление оценки: семинары Марии 3 → 5");
        var corrected = _pos.CorrectEvaluation(mariaSeminars.Id, 5, "доработала");
        Console.WriteLine($"  ~ {ConsoleMenu.Describe(corrected)}");

        Step("6. Итог (у Марии нет экзамена — правила считают по-разному)");
        foreach (var policy in _pos.GetGradingPolicies())
        {
            ConsoleMenu.PrintFinalGrades(_pos.CalculateFinalGrades(policy.Name));
        }

        Step("7. Экспорт итога (strict, csv)");
        const string path = "final-grades.csv";
        using (var writer = new StreamWriter(path))
        {
            _pos.ExportFinalGrades("strict", "csv", writer);
        }
        Console.WriteLine($"  Файл: {Path.GetFullPath(path)}");
        Console.WriteLine(File.ReadAllText(path));
    }

    private static void Step(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"✅ {title} ✅");
    }

    private static void TryViolation(string what, Action action)
    {
        try
        {
            action();
            Console.WriteLine($"  !! {what}: правило НЕ сработало");
        }
        catch (DomainException ex)
        {
            Console.WriteLine($"  x {what}: {ex.Message}");
        }
    }
}
