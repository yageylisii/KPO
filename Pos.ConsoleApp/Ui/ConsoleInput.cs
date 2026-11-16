using System.Globalization;

namespace Pos.ConsoleApp.Ui;

// Чтение ввода с повтором при ошибке
public sealed class ConsoleInput
{   

    public string ReadString(string prompt, bool allowEmpty = false)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var line = Console.ReadLine()?.Trim() ?? "";

            if (allowEmpty || line.Length > 0)
            {
                return line;
            }

            Console.WriteLine("  Значение не может быть пустым.");
        }
    }

    public decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            var line = ReadString(prompt).Replace(',', '.');

            if (decimal.TryParse(line, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            Console.WriteLine("  Введите число, например 7 или 7.5.");
        }
    }

    // Выбор элемента из пронумерованного списка: null — список пуст или отмена (0)

    // Один метод работает со списком любых объектов: студентов, критериев, оценок, правил итога, форматов
    public T? Choose<T>(string title, IReadOnlyList<T> items, Func<T, string> describe) where T : class
    {
        if (items.Count == 0)
        {
            Console.WriteLine($"  {title}: список пуст.");
            return null;
        }

        Console.WriteLine($"{title}:");
        for (var i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {describe(items[i])}");
        }

        while (true)
        {
            var line = ReadString("Номер (0 — отмена)");

            if (int.TryParse(line, out var n) && n >= 0 && n <= items.Count)
            {
                return n == 0 ? null : items[n - 1];
            }

            Console.WriteLine($"  Введите число от 0 до {items.Count}.");
        }
    }
}
