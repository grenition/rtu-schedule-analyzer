using Project.Core.ViewModels;

namespace Project.Core.Interfaces.Services;

using Entities;

public interface ILessonsService
{
    Task<IEnumerable<Lesson>> SearchAllLessons(string searchKey, CancellationToken cancellationToken);
    Task<IEnumerable<SearchInconvenienceViewModel>> SearchInconveniences(string searchKey, CancellationToken cancellationToken);
}
