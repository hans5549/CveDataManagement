namespace WebConsole.Models;

public class CveProduct
{
    public string? Vendor { get; set; }
    public string? Product { get; set; }
    public List<string>? CveIds { get; set; }
}