using Microsoft.EntityFrameworkCore;

namespace Project.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
}
