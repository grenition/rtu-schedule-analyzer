using Project.Core.Entities;
using Project.Core.Interfaces.Repositories;
using Project.Core.Interfaces.Services;

namespace Project.Core.Services;

public class LessonsService(
    IScheduleRepository scheduleRepository,
    IInconveniencesRepository inconveniencesRepository) : ILessonsService
{
    public const double SearchLifetimeHours = 1.0;

    public async Task<IEnumerable<Lesson>> SearchAllLessons(string searchKey, CancellationToken cancellationToken) =>
        await scheduleRepository
            .GetLessons(searchKey, cancellationToken)
            .OrderBy(x => x.Start)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<SearchInconvenience>> SearchInconveniences(string searchKey, CancellationToken cancellationToken)
    {
        var lessons = await SearchAllLessons(searchKey, cancellationToken);
        var inconveniences = new List<SearchInconvenience>();

        var cachedInconvenience = await inconveniencesRepository.GetFirst(searchKey);

        if (cachedInconvenience != null && (DateTime.UtcNow - cachedInconvenience.SearchTime).TotalHours <= SearchLifetimeHours)
            return await inconveniencesRepository.GetInconviences(searchKey).ToListAsync();

        Lesson? previousLesson = null;

        foreach (var lesson in lessons.OrderBy(x => x.Start))
        {
            if (previousLesson == null || previousLesson.Start.Date != lesson.Start.Date)
            {
                previousLesson = lesson;
                continue;
            }

            if ((lesson.Start - previousLesson.End).TotalHours >= 1.0)
                inconveniences.Add(new SearchInconvenience()
                {
                    Id = Guid.NewGuid().ToString(),
                    FromLessonId = previousLesson.Id,
                    ToLessonId = lesson.Id,
                    Type = "WINDOW",
                    Date = DateOnly.FromDateTime(lesson.Start.DateTime),
                    SearchKey = searchKey,
                    SearchTime = DateTime.UtcNow
                });

            if (previousLesson.Campus != lesson.Campus)
                inconveniences.Add(new SearchInconvenience()
                {
                    Id = Guid.NewGuid().ToString(),
                    FromLessonId = previousLesson.Id,
                    ToLessonId = lesson.Id,
                    Type = "DIFF_CAMPUS",
                    Date = DateOnly.FromDateTime(lesson.Start.DateTime),
                    SearchKey = searchKey,
                    SearchTime = DateTime.UtcNow
                });

            previousLesson = lesson;
        }

        if (cachedInconvenience != null)
            await inconveniencesRepository.RemoveInconviences(searchKey);

        await inconveniencesRepository.AddRange(inconveniences);

        return inconveniences;
    }

    public async Task ClearCache(string searchKey)
    {
        await inconveniencesRepository.RemoveInconviences(searchKey);
    }
}
