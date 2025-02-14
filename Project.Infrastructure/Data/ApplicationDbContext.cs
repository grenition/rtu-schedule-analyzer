using Microsoft.EntityFrameworkCore;
using Project.Core.Entities;

namespace Project.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<SearchInconvenience> Inconveniences { get; private set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
}
