namespace Project.Core.Entities;

public class SearchInconvenience : Inconvenience
{
    public string? SearchKey { get; set; }
    public DateTime SearchTime { get; set; }
}
