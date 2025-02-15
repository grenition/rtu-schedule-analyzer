namespace Project.API.Extensions;

using Core.Interfaces.Services;
using Core.Services;

public static class ProjectCoreExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ILessonsService, LessonsService>();

        return services;
    }
}
