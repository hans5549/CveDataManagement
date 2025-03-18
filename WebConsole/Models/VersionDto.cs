namespace WebConsole.Models;

public class VersionDto
{
    public int VersionId { get; set; }
    public int AffectedId { get; set; }
    public string? VersionValue { get; set; }
    public string? Status { get; set; }
    public string? LessThanOrEqual { get; set; }
    public string? VersionType { get; set; }
} 