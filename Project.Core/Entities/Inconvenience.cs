namespace Project.Core.Entities;

public class Inconvenience
{
    public string? FromLessonId { get; set; }
    public Lesson? FromLesson { get; set; }
    public string? ToLessonId { get; set; }
    public Lesson? ToLesson { get; set; }
    public string? Type { get; set; }
    public DateOnly Date { get; set; }
}
