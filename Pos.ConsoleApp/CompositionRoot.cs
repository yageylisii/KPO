using Microsoft.Extensions.DependencyInjection;
using Pos.ConsoleApp.Ui;
using Pos.Infrastructure;
using Pos.UseCases;

namespace Pos.ConsoleApp;

// Composition root: единственное место, где все слои встречаются
// и граф зависимостей собирается DI-контейнером
public static class CompositionRoot
{
    private static IServiceProvider? _services;
    public static IServiceProvider Services => _services ??= CreateServiceProvider(); // если services = null => создаем контейнер
    
    private static IServiceProvider CreateServiceProvider() // строим контейнер
    {
        var services = new ServiceCollection();   // пустой список регистраций
        services.AddUseCases();                   // + фабрика, калькулятор, 2 стратегии, TimeProvider, фасад
        services.AddInfrastructure();             // + 3 репозитория, CSV-экспортёр
        services.AddSingleton<ConsoleInput>();    // + классы UI
        services.AddSingleton<DemoScenario>();
        services.AddSingleton<ConsoleMenu>();

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true, // ошибки регистрации видны сразу при старте
        });
    }
}
