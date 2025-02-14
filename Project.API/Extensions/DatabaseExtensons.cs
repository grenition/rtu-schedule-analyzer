using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;

namespace Project.API.Extensions;

public static class DatabaseExtensons
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            var username = Environment.GetEnvironmentVariable("POSTGRES_USER");
            var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new InvalidOperationException("Cannot create a connection string because POSTGRES_USER or POSTGRES_PASSWORD environment variables is not assigned");

            connectionString = $"Host=db;Port=5432;Database=main;Username={username};Password={password}";
        }
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        return services;
    }
}
