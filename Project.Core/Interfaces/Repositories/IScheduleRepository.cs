namespace Project.Core.Interfaces.Repositories;

using Entities;

public interface IScheduleRepository
{
    IAsyncEnumerable<Lesson> GetLessons(string searchKey, CancellationToken cancellationToken);
}
