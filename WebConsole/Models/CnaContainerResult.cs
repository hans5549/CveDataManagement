namespace WebConsole.Models;

public class CnaContainerResult
{
    public int? ContainersId { get; set; }
    public int? CnaId { get; set; }
    public string? Title { get; set; } = string.Empty;
    public string? OrgId { get; set; } = string.Empty;
    public string? ShortName { get; set; } = string.Empty;
    public DateTime? DateUpdated { get; set; }
}