using Microsoft.Extensions.DependencyInjection;
using Pos.Infrastructure.Export;
using Pos.Infrastructure.Persistence;
using Pos.UseCases.Ports;

namespace Pos.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Хранилище. Замена на JSON-файлы = новые классы + эти три строки
        services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
        services.AddSingleton<IRubricRepository, InMemoryRubricRepository>();
        services.AddSingleton<IEvaluationRepository, InMemoryEvaluationRepository>();

        // Экспорт итогов
        services.AddSingleton<IFinalGradeExporter, CsvFinalGradeExporter>();

        return services;
    }
}
