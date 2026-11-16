using Microsoft.Extensions.DependencyInjection;
using Pos.ConsoleApp;
using Pos.ConsoleApp.Ui;

// переводим консоль в UTF-8
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

// dotnet run -- --demo сразу прогнать сценарий без меню
if (args.Contains("--demo"))
{   
    CompositionRoot.Services.GetRequiredService<DemoScenario>().Run();
    return;
}
// CompositionRoot.Services создаем правила
// GetRequiredService создаем объекты по этим правилам
var menu = CompositionRoot.Services.GetRequiredService<ConsoleMenu>();
// тут в menu хранятся уже все зависимости в контейнере
menu.Run();
