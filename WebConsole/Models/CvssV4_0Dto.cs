namespace WebConsole.Models;

public class CvssV4_0Dto
{
    public string? Version { get; set; } = string.Empty;
    public double? BaseScore { get; set; }
    public string? VectorString { get; set; } = string.Empty;
    public string? BaseSeverity { get; set; } = string.Empty;
}