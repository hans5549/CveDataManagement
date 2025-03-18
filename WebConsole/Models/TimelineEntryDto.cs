namespace WebConsole.Models;

public class TimelineEntryDto
{
    public DateTime Time { get; set; }
    public string? Language { get; set; } = string.Empty;
    public string? Value { get; set; } =  string.Empty;
}