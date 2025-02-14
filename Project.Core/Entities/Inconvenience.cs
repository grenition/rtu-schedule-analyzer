using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities;

public class Inconvenience
{
    [Key]
    public string? Id { get; set; }
    public string? FromLessonId { get; set; }
    public string? ToLessonId { get; set; }
    public string? Type { get; set; }
    public DateOnly Date { get; set; }
}
