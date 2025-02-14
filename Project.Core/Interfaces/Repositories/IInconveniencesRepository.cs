using Project.Core.Entities;

namespace Project.Core.Interfaces.Repositories;

public interface IInconveniencesRepository
{
    Task<SearchInconvenience?> GetFirst(string searchKey);
    IAsyncEnumerable<SearchInconvenience> GetInconviences(string searchKey);
    Task RemoveInconviences(string searchKey);
    Task AddRange(IEnumerable<SearchInconvenience> inconveniences);
}
