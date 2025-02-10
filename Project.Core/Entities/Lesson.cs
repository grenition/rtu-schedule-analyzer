namespace Project.Core.Entities;

public class Lesson
{
    public string Id { get; set; } = string.Empty;
    public string Discipline { get; set; } = string.Empty;
    public string? Campus { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
