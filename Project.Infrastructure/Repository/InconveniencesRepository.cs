using Microsoft.EntityFrameworkCore;
using Project.Core.Entities;
using Project.Core.Interfaces.Repositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repository;

public class InconveniencesRepository(ApplicationDbContext dbContext) : IInconveniencesRepository
{
    public Task<SearchInconvenience?> GetFirst(string searchKey)
    {
        return dbContext.Inconveniences
            .Where(x => x.SearchKey == searchKey)
            .FirstOrDefaultAsync();
    }
    public IAsyncEnumerable<SearchInconvenience> GetInconviences(string searchKey)
    {
        return dbContext.Inconveniences
            .Where(x => x.SearchKey == searchKey)
            .AsAsyncEnumerable();
    }
    public async Task RemoveInconviences(string searchKey)
    {
        await dbContext.Inconveniences
            .Where(x => x.SearchKey == searchKey)
            .ExecuteDeleteAsync();
    }
    public async Task AddRange(IEnumerable<SearchInconvenience> inconveniences)
    {
        await dbContext.Inconveniences.AddRangeAsync(inconveniences);
        await dbContext.SaveChangesAsync();
    }
}
