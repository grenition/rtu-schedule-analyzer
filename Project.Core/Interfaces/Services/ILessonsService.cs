using Project.Core.Entities;

namespace Project.Core.Interfaces.Services;

public interface ILessonsService
{
    Task<IEnumerable<Lesson>> SearchAllLessons(string searchKey, CancellationToken cancellationToken);
    Task<IEnumerable<SearchInconvenience>> SearchInconveniences(string searchKey, CancellationToken cancellationToken);
    Task ClearCache(string searchKey);
}
