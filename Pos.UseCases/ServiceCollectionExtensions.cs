using Microsoft.Extensions.DependencyInjection;
using Pos.Entities.Evaluations;
using Pos.Entities.Grading;
using Pos.Entities.Grading.Strategies;

namespace Pos.UseCases;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        // Домен: фабрика и Domain Service не имеют состояния — Singleton
        services.AddSingleton<EvaluationFactory>();
        services.AddSingleton<GradeCalculator>();

        // Правила итога (Strategy). Новое правило = новый класс + одна строка здесь
        services.AddSingleton<IGradingStrategy, StrictWeightedStrategy>();
        services.AddSingleton<IGradingStrategy, ProgressWeightedStrategy>();

        services.AddSingleton(TimeProvider.System);

        // Фасад сценариев
        services.AddSingleton<IGradingFacade, GradingFacade>();

        return services;
    }
}
