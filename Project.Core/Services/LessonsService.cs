namespace Project.Core.Services;

using Entities;
using Interfaces.Repositories;
using Interfaces.Services;

public class LessonsService(IScheduleRepository scheduleRepository) : ILessonsService
{
    public async Task<IEnumerable<Lesson>> GetAllLessons(string searchKey, CancellationToken cancellationToken) =>
        await scheduleRepository.GetLessons(searchKey, cancellationToken).ToListAsync(cancellationToken);

    public IEnumerable<Inconvenience> GetInconveniences(IEnumerable<Lesson>? lessons)
    {
        if(lessons == null)
            yield break;

        Lesson? previousLesson = null;
        
        foreach (var lesson in lessons.OrderBy(x => x.Start))
        {
            if (previousLesson == null || previousLesson.Start.Date != lesson.Start.Date)
            {
                previousLesson = lesson;
                continue;
            }

            if ((lesson.Start - previousLesson.End).TotalHours >= 1.0)
                yield return new Inconvenience()
                {
                    FromLessonId = previousLesson.Id,
                    FromLesson = previousLesson,
                    ToLessonId = lesson.Id,
                    ToLesson = lesson,
                    Type = "WINDOW",
                };

            if (previousLesson.Campus != lesson.Campus)
                yield return new Inconvenience()
                {
                    FromLessonId = previousLesson.Id,
                    FromLesson = previousLesson,
                    ToLessonId = lesson.Id,
                    ToLesson = lesson,
                    Type = "DIFF_CAMPUS",
                };
            
            previousLesson = lesson;
        }
    }
}
