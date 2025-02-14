using Project.Core.Interfaces.Repositories;
using Project.Infrastructure.Repository;

namespace Project.API.Extensions;

public static class ProjectInfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IInconveniencesRepository, InconveniencesRepository>();

        return services;
    }
}
