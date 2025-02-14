using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Project.Infrastructure.Helpers;

namespace Project.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            if(File.Exists("../.env")) Env.Load("../.env");
            
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = DatabasesHelper.GetConnectionString();

            optionsBuilder.UseNpgsql(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
