using Project.Core.Entities;

namespace Project.Core.ViewModels;

public class InconvenienceViewModel
{
    public string? FromLessonId { get; set; }
    public string? ToLessonId { get; set; }
    public string? Type { get; set; }
    public DateOnly Date { get; set; }

    public InconvenienceViewModel(Inconvenience inconvenience)
    {
        FromLessonId = inconvenience.FromLessonId;
        ToLessonId = inconvenience.ToLessonId;
        Type = inconvenience.Type;
        Date = inconvenience.Date;
    }
}
