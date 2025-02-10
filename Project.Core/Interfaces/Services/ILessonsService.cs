namespace Project.Core.Interfaces.Services;

using Entities;

public interface ILessonsService
{
    Task<IEnumerable<Lesson>> GetAllLessons(string searchKey, CancellationToken cancellationToken);
    IEnumerable<Inconvenience> GetInconveniences(IEnumerable<Lesson>? lessons);
}
