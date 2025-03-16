namespace WebConsole.Models;

public class MappingResult
{
    public bool IsSuccess { get; set; }
    public string OriginalFilePath { get; set; }
    public Cve.RootCve? RootCve { get; set; }
}