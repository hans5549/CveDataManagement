namespace WebConsole.Models;

public class ProblemTypeDescriptionDto
{
    public int ProblemTypeId { get; set; }
    public string? CveId { get; set; }
    public string? CweId { get; set; }
    public string? Description { get; set; }
    public string? Language { get; set; }
    public string? Type { get; set; }
}