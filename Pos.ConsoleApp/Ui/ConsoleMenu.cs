using Pos.Entities.Common;
using Pos.UseCases;
using Pos.UseCases.Dto;

namespace Pos.ConsoleApp.Ui;

// Консольный UI. Работает только через IGradingFacade и DTO
public sealed class ConsoleMenu
{
    private readonly IGradingFacade _pos;
    private readonly ConsoleInput _input;
    private readonly DemoScenario _demo;

    public ConsoleMenu(IGradingFacade pos, ConsoleInput input, DemoScenario demo)
    {
        _pos = pos;
        _input = input;
        _demo = demo;
    }
    // запуск программы после создания DI- контейнера
    public void Run()
    {
        var actions = new (string Key, string Title, Action Action)[]
        {
            ("1", "Добавить студента", AddStudent),
            ("2", "Список студентов\n", ShowStudents),
            ("3", "Добавить критерий", AddCriterion),
            ("4", "Рубрикатор", ShowCriteria),
            ("5", "Удалить критерий\n", RemoveCriterion),
            ("6", "Выставить оценку", GiveEvaluation),
            ("7", "Исправить оценку", CorrectEvaluation),
            ("8", "Отменить оценку", CancelEvaluation),
            ("9", "Список оценок\n", ShowEvaluations),
            ("10", "Итоги", ShowFinalGrades),
            ("11", "Экспорт итогов", ExportFinalGrades),
            ("d", "Демо-сценарий", _demo.Run),
        };

        while (true)
        {   
            // печатаем меню
            Console.WriteLine();
            Console.WriteLine("🔱 ПОС — преподавательская оценка студентов 🔱");
            foreach (var (key, title, _) in actions)
            {
                Console.WriteLine($"{key,3}. {title}");
            }
            Console.WriteLine("  0. Выход");

            var choice = _input.ReadString(">");
            if (choice == "0")
            {
                return;
            }

            var action = actions.FirstOrDefault(a => a.Key == choice).Action;
            if (action is null)
            {
                Console.WriteLine("Нет такого пункта.");
                continue;
            }

            Execute(action);
        }
    }

    // Ошибки домена и сценариев показываем пользователю, а не роняем программу
    private static void Execute(Action action)
    {
        try
        {
            action();
        }
        catch (DomainException ex)
        {
            Console.WriteLine($"[Правило нарушено] {ex.Message}");
        }
        catch (UseCaseException ex)
        {
            Console.WriteLine($"[Ошибка] {ex.Message}");
        }
    }

    private void AddStudent()
    {
        var student = _pos.AddStudent(_input.ReadString("ФИО"));
        Console.WriteLine($"Добавлен: {student.FullName}");
    }

    private void ShowStudents()
    {
        var students = _pos.GetStudents();
        Console.WriteLine(students.Count == 0 ? "Студентов нет." : "Студенты:");
        foreach (var s in students)
        {
            Console.WriteLine($"  - {s.FullName}");
        }
    }

    private void AddCriterion()
    {
        var name = _input.ReadString("Название");
        var max = _input.ReadDecimal("Максимальный балл");
        var weight = _input.ReadDecimal("Вес");
        var criterion = _pos.AddCriterion(name, max, weight);
        Console.WriteLine($"Добавлен критерий: {Describe(criterion)}");
    }

    private void ShowCriteria()
    {
        var criteria = _pos.GetCriteria();
        Console.WriteLine(criteria.Count == 0 ? "Рубрикатор пуст." : "Рубрикатор:");
        foreach (var c in criteria)
        {
            Console.WriteLine($"  - {Describe(c)}");
        }
    }

    private void RemoveCriterion()
    {
        var criterion = _input.Choose("Критерии", _pos.GetCriteria(), Describe);
        if (criterion is null) return;

        _pos.RemoveCriterion(criterion.Id);
        Console.WriteLine($"Критерий «{criterion.Name}» удалён.");
    }

    private void GiveEvaluation()
    {
        var student = _input.Choose("Студенты", _pos.GetStudents(), s => s.FullName);
        if (student is null) return;

        var criterion = _input.Choose("Критерии", _pos.GetCriteria(), Describe);
        if (criterion is null) return;

        var score = _input.ReadDecimal($"Балл (0..{criterion.MaxScore:0.##})");
        var comment = _input.ReadString("Комментарий (Enter — без комментария)", allowEmpty: true);

        var evaluation = _pos.GiveEvaluation(student.Id, criterion.Id, score, comment);
        Console.WriteLine($"Выставлено: {Describe(evaluation)}");
    }

    private void CorrectEvaluation()
    {
        var evaluation = _input.Choose("Оценки", _pos.GetEvaluations(), Describe);
        if (evaluation is null) return;

        var score = _input.ReadDecimal($"Новый балл (0..{evaluation.MaxScore:0.##})");
        var comment = _input.ReadString("Комментарий (Enter — оставить прежний)", allowEmpty: true);

        var corrected = _pos.CorrectEvaluation(evaluation.Id, score, comment);
        Console.WriteLine($"Исправлено: {Describe(corrected)}");
    }

    private void CancelEvaluation()
    {
        var evaluation = _input.Choose("Оценки", _pos.GetEvaluations(), Describe);
        if (evaluation is null) return;

        _pos.CancelEvaluation(evaluation.Id);
        Console.WriteLine("Оценка отменена.");
    }

    private void ShowEvaluations()
    {
        var evaluations = _pos.GetEvaluations();
        Console.WriteLine(evaluations.Count == 0 ? "Оценок нет." : "Оценки:");
        foreach (var e in evaluations)
        {
            Console.WriteLine($"  - {Describe(e)}");
        }
    }

    private void ShowFinalGrades()
    {
        var policy = ChoosePolicy();
        if (policy is null) return;

        PrintFinalGrades(_pos.CalculateFinalGrades(policy.Name));
    }

    private void ExportFinalGrades()
    {
        var policy = ChoosePolicy();
        if (policy is null) return;

        var format = _input.Choose("Формат", _pos.GetExportFormats(), f => f);
        if (format is null) return;

        var path = _input.ReadString($"Файл (Enter — final-grades.{format})", allowEmpty: true);
        if (path.Length == 0) path = $"final-grades.{format}";

        using (var writer = new StreamWriter(path))
        {
            _pos.ExportFinalGrades(policy.Name, format, writer);
        }

        Console.WriteLine($"Итоги выгружены в {Path.GetFullPath(path)}");
    }

    private GradingPolicyDto? ChoosePolicy() =>
        _input.Choose("Правило итога", _pos.GetGradingPolicies(), p => $"{p.Name} — {p.Description}");

    internal static void PrintFinalGrades(IReadOnlyList<FinalGradeDto> grades)
    {
        if (grades.Count == 0)
        {
            Console.WriteLine("Студентов нет.");
            return;
        }

        Console.WriteLine($"Итоги (правило: {grades[0].PolicyName}):");
        foreach (var g in grades)
        {
            var parts = g.Breakdown.Select(b =>
                $"{b.CriterionName} {(b.Score is null ? "—" : b.Score.Value.ToString("0.##"))}/{b.MaxScore:0.##}");
            Console.WriteLine($"  {g.StudentName,-20} {g.Value,6:0.00}   [{string.Join(", ", parts)}]");
        }
    }

    internal static string Describe(CriterionDto c) => $"{c.Name} (max {c.MaxScore:0.##}, вес {c.Weight:0.##})";

    internal static string Describe(EvaluationDto e) =>
        $"{e.StudentName} — {e.CriterionName}: {e.Score:0.##}/{e.MaxScore:0.##} ({e.Date:dd.MM.yyyy})"
        + (e.Comment is null ? "" : $" «{e.Comment}»");
}
