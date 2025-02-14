using Project.Core.Entities;
using Project.Core.Interfaces.Repositories;
using Project.Core.Interfaces.Services;

namespace Project.Core.Services;

public class LessonsService(IScheduleRepository scheduleRepository) : ILessonsService
{
    public async Task<IEnumerable<Lesson>> SearchAllLessons(string searchKey, CancellationToken cancellationToken) =>
        await scheduleRepository
            .GetLessons(searchKey, cancellationToken)
            .OrderBy(x => x.Start)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<SearchInconvenience>> SearchInconveniences(string searchKey, CancellationToken cancellationToken)
    {
        var lessons = await SearchAllLessons(searchKey, cancellationToken);

        var inconvenieces = new List<SearchInconvenience>();
        Lesson? previousLesson = null;
        
        foreach (var lesson in lessons.OrderBy(x => x.Start))
        {
            if (previousLesson == null || previousLesson.Start.Date != lesson.Start.Date)
            {
                previousLesson = lesson;
                continue;
            }

            if ((lesson.Start - previousLesson.End).TotalHours >= 1.0)
                inconvenieces.Add(new SearchInconvenience()
                {
                    Id = Guid.NewGuid().ToString(),
                    FromLessonId = previousLesson.Id,
                    ToLessonId = lesson.Id,
                    Type = "WINDOW",
                    Date = DateOnly.FromDateTime(lesson.Start.Date),
                    SearchKey = searchKey,
                    SearchTime = DateTime.Now
                });

            if (previousLesson.Campus != lesson.Campus)
                inconvenieces.Add(new SearchInconvenience()
                {
                    Id = Guid.NewGuid().ToString(),
                    FromLessonId = previousLesson.Id,
                    ToLessonId = lesson.Id,
                    Type = "DIFF_CAMPUS",
                    Date = DateOnly.FromDateTime(lesson.Start.Date),
                    SearchKey = searchKey,
                    SearchTime = DateTime.Now
                });
            
            previousLesson = lesson;
        }

        return inconvenieces;
    }
}
