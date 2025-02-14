using Project.Core.Entities;
using Project.Core.Interfaces.Repositories;
using Project.Core.Interfaces.Services;
using Project.Core.ViewModels;

namespace Project.Core.Services;

public class LessonsService(IScheduleRepository scheduleRepository) : ILessonsService
{
    public async Task<IEnumerable<Lesson>> SearchAllLessons(string searchKey, CancellationToken cancellationToken) =>
        await scheduleRepository
            .GetLessons(searchKey, cancellationToken)
            .OrderBy(x => x.Start)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<SearchInconvenienceViewModel>> SearchInconveniences(string searchKey, CancellationToken cancellationToken)
    {
        var lessons = await SearchAllLessons(searchKey, cancellationToken);

        var inconvenieces = new List<SearchInconvenienceViewModel>();
        Lesson? previousLesson = null;
        
        foreach (var lesson in lessons.OrderBy(x => x.Start))
        {
            if (previousLesson == null || previousLesson.Start.Date != lesson.Start.Date)
            {
                previousLesson = lesson;
                continue;
            }

            if ((lesson.Start - previousLesson.End).TotalHours >= 1.0)
                inconvenieces.Add(new SearchInconvenienceViewModel(new()
                {
                    FromLessonId = previousLesson.Id,
                    FromLesson = previousLesson,
                    ToLessonId = lesson.Id,
                    ToLesson = lesson,
                    Type = "WINDOW",
                    Date = DateOnly.FromDateTime(lesson.Start.Date)
                }, searchKey));

            if (previousLesson.Campus != lesson.Campus)
                inconvenieces.Add(new SearchInconvenienceViewModel(new()
                {
                    FromLessonId = previousLesson.Id,
                    FromLesson = previousLesson,
                    ToLessonId = lesson.Id,
                    ToLesson = lesson,
                    Type = "DIFF_CAMPUS",
                    Date = DateOnly.FromDateTime(lesson.Start.Date)
                }, searchKey));
            
            previousLesson = lesson;
        }

        return inconvenieces;
    }
}
