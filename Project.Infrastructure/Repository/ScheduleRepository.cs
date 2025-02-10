using System.Runtime.CompilerServices;

namespace Project.Infrastructure.Repository;

using Core.Entities;
using Core.Interfaces.Repositories;
using RTU_TC.RTUScheduleClient;
using RTU_TC.RTUScheduleClient.ICal;

public class ScheduleRepository : IScheduleRepository
{
    private readonly IRTUScheduleClient _scheduleClient;

    public ScheduleRepository(IHttpClientFactory httpClientFactory)
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://schedule-of.mirea.ru");
        httpClient.DefaultRequestHeaders.Add(HttpSearchICalContentRTUSchedule.ClientNameHeaderKey, "schedule-repository");
        _scheduleClient = new HttpSearchICalContentRTUSchedule(httpClient);
    }

    public async IAsyncEnumerable<Lesson> GetLessons(string searchKey, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var schedules = _scheduleClient.GetAllSchedulesAsync(searchKey);

        await foreach (var schedule in schedules.WithCancellation(cancellationToken))
        {
            var calendar = await schedule.GetCalendarAsync();
            foreach (var lesson in calendar.GetAllLessons())
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                yield return new Lesson()
                {
                    Id = lesson.Id,
                    Discipline = lesson.Discipline,
                    Campus = lesson.Auditoriums.FirstOrDefault()?.Campus,
                    Start = lesson.Start,
                    End = lesson.End
                };
            }
        }
    }
}
