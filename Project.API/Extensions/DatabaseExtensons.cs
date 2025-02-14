using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Helpers;

namespace Project.API.Extensions;

public static class DatabaseExtensons
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        var connectionString = DatabasesHelper.GetConnectionString();
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        return services;
    }
}
